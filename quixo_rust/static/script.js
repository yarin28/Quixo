class QuixoGame {
    constructor() {
        this.API_BASE = '';
        this.boardElement = document.getElementById('board');
        this.currentPlayerElement = document.getElementById('currentPlayer');
        this.winnerElement = document.getElementById('winner');
        this.loadingElement = document.getElementById('loading');
        this.makeMoveBtn = document.getElementById('makeMove');
        this.newGameBtn = document.getElementById('newGame');
        this.refreshBtn = document.getElementById('refresh');
        this.possibleMoves = [];

        this.bindEvents();
        this.refreshStatus();
    }

    bindEvents() {
        this.makeMoveBtn.addEventListener('click', () => this.makeMove());
        this.newGameBtn.addEventListener('click', () => this.newGame());
        this.refreshBtn.addEventListener('click', () => this.refreshStatus());

        // Allow Enter key to make move
        document.addEventListener('keypress', (e) => {
            if (e.key === 'Enter') {
                this.makeMove();
            }
        });
    }

    async apiCall(endpoint, method = 'GET', data = null) {
        const config = {
            method,
            headers: {
                'Content-Type': 'application/json',
            },
        };

        if (data) {
            config.body = JSON.stringify(data);
        }

        try {
            const response = await fetch(`${this.API_BASE}${endpoint}`, config);

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(`HTTP ${response.status}: ${errorText}`);
            }

            return await response.json();
        } catch (error) {
            console.error('API call failed:', error);
            this.showError(`API Error: ${error.message}`);
            throw error;
        }
    }

    async newGame() {
        try {
            this.showLoading('Starting new game...');
            const data = await this.apiCall('/start', 'POST');
            this.updateGameState(data);
            this.clearAllSelections();
            await this.fetchPossibleMoves();
            this.showSuccess('New game started!');
            this.hideLoading();
        } catch (error) {
            this.showError('Failed to start new game');
        }
    }

    async makeMove() {
        const row = parseInt(document.getElementById('row').value);
        const col = parseInt(document.getElementById('col').value);
        const direction = document.getElementById('direction').value;

        // Validate input
        if (row < 0 || row > 4 || col < 0 || col > 4) {
            this.showError('Row and column must be between 0 and 4');
            return;
        }

        if (!this.isValidMove(row, col)) {
            this.showError('This is not a valid move. Please select a highlighted cell.');
            return;
        }

        try {
            this.showLoading('Making move...');
            this.makeMoveBtn.disabled = true;

            const moveData = { row, col, direction };
            const data = await this.apiCall('/move', 'POST', moveData);

            this.updateGameState(data);
            this.clearAllSelections();
            this.showSuccess('Move successful!');
            this.hideLoading();
        } catch (error) {
            this.showError('Move failed: ' + error.message);
        } finally {
            this.makeMoveBtn.disabled = false;
        }
    }

    async refreshStatus() {
        try {
            this.showLoading('Loading game status...');
            const data = await this.apiCall('/status');
            this.updateGameState(data);
            await this.fetchPossibleMoves();
            this.showSuccess('Game loaded successfully!');
        } catch (error) {
            console.error('Failed to refresh status:', error);
            this.showError('Failed to load game. Please check if the server is running.');
            // Try to start a new game if status fails
            setTimeout(() => {
                this.showLoading('Starting new game...');
                this.newGame();
            }, 2000);
        }
    }

    async fetchPossibleMoves() {
        try {
            const data = await this.apiCall('/possible-moves');
            this.possibleMoves = data.moves;
            this.updatePossibleMovesDisplay();
        } catch (error) {
            console.error('Failed to fetch possible moves:', error);
            this.possibleMoves = [];
            this.showError('Could not load move suggestions, but you can still play!');
        }
    }

    updateGameState(data) {
        this.renderBoard(data.board);
        this.updateCurrentPlayer(data.current_player);

        if (data.winner) {
            this.showWinner(data.winner);
        } else {
            this.hideWinner();
        }
    }

    updatePossibleMovesDisplay() {
        // Clear previous move indicators
        document.querySelectorAll('.cell').forEach(cell => {
            cell.classList.remove('valid-move');
        });

        // Add indicators for valid moves
        this.possibleMoves.forEach(move => {
            const cell = document.querySelector(`[data-row="${move.row}"][data-col="${move.col}"]`);
            if (cell) {
                cell.classList.add('valid-move');
            }
        });

        // Update move controls with available options
        this.updateMoveControls();
    }

    updateMoveControls() {
        const rowSelect = document.getElementById('row');
        const colSelect = document.getElementById('col');

        // Clear existing options
        rowSelect.innerHTML = '';
        colSelect.innerHTML = '';

        // Get unique positions from possible moves
        const uniquePositions = [...new Set(this.possibleMoves.map(m => `${m.row},${m.col}`))];

        if (uniquePositions.length > 0) {
            uniquePositions.forEach(pos => {
                const [row, col] = pos.split(',').map(Number);
                rowSelect.innerHTML += `<option value="${row}">${row}</option>`;
                colSelect.innerHTML += `<option value="${col}">${col}</option>`;
            });

            // Set default values
            const [defaultRow, defaultCol] = uniquePositions[0].split(',').map(Number);
            rowSelect.value = defaultRow;
            colSelect.value = defaultCol;
        }
    }

    renderBoard(boardString) {
        const rows = boardString.trim().split('\n');
        this.boardElement.innerHTML = '';

        rows.forEach((row, rowIndex) => {
            const cells = row.trim().split(/\s+/);
            cells.forEach((cell, colIndex) => {
                const cellElement = document.createElement('div');
                cellElement.className = `cell ${this.getCellClass(cell)}`;
                cellElement.textContent = cell === '.' ? '' : cell;
                cellElement.dataset.row = rowIndex;
                cellElement.dataset.col = colIndex;



                // Add single click handler that handles both piece selection and destination clicks
                cellElement.addEventListener('click', (e) => {
                    console.log('Cell clicked:', { row: rowIndex, col: colIndex, classes: cellElement.className });

                    // First, check if this is a destination highlight click
                    if (cellElement.classList.contains('destination-highlight')) {
                        e.stopPropagation();
                        e.preventDefault();
                        const direction = cellElement.dataset.moveDirection;
                        const sourceRow = parseInt(cellElement.dataset.sourceRow);
                        const sourceCol = parseInt(cellElement.dataset.sourceCol);

                        console.log('🎯 Destination clicked:', { sourceRow, sourceCol, direction });

                        // Auto-fill the form and make the move
                        document.getElementById('row').value = sourceRow;
                        document.getElementById('col').value = sourceCol;
                        document.getElementById('direction').value = direction;

                        // Make the move immediately
                        this.makeMove();
                        return; // Don't process as piece selection
                    }

                    // Otherwise, handle as piece selection
                    if (this.isValidMove(rowIndex, colIndex)) {
                        console.log('🎪 Piece selected:', { row: rowIndex, col: colIndex });
                        this.clearAllSelections();
                        document.getElementById('row').value = rowIndex;
                        document.getElementById('col').value = colIndex;
                        this.highlightCell(cellElement);
                        this.showDestinationHighlights(cellElement, rowIndex, colIndex);
                    }
                });

                this.boardElement.appendChild(cellElement);
            });
        });
    }

    getCellClass(cell) {
        switch (cell) {
            case 'X': return 'X';
            case 'O': return 'O';
            case '.': return 'Empty';
            default: return 'Empty';
        }
    }

    isPeriphery(row, col) {
        return row === 0 || row === 4 || col === 0 || col === 4;
    }

    isValidMove(row, col) {
        return this.possibleMoves.some(move => move.row === row && move.col === col);
    }



    showDestinationHighlights(cellElement, row, col) {
        // Clear all previous highlights and selections
        this.clearAllSelections();

        // Mark the selected cell
        cellElement.classList.add('selected');

        // Calculate and highlight destination squares
        const cellMoves = this.possibleMoves.filter(move => move.row === row && move.col === col);
        cellMoves.forEach(move => {
            const destination = this.calculateDestination(row, col, move.direction);
            if (destination) {
                const destCell = document.querySelector(`[data-row="${destination.row}"][data-col="${destination.col}"]`);
                if (destCell) {
                    destCell.classList.add('destination-highlight');
                    destCell.dataset.moveDirection = move.direction;
                    destCell.dataset.sourceRow = row;
                    destCell.dataset.sourceCol = col;
                }
            }
        });
    }

    calculateDestination(row, col, direction) {
        switch (direction) {
            case 'up':
                return { row: 0, col }; // Top of column
            case 'down':
                return { row: 4, col }; // Bottom of column
            case 'left':
                return { row, col: 0 }; // Left of row
            case 'right':
                return { row, col: 4 }; // Right of row
            default:
                return null;
        }
    }



    updateCurrentPlayer(player) {
        this.currentPlayerElement.textContent = player;
    }

    showWinner(winner) {
        this.winnerElement.textContent = `${winner} wins!`;
        this.winnerElement.style.display = 'block';
        this.makeMoveBtn.disabled = true;
    }

    hideWinner() {
        this.winnerElement.style.display = 'none';
        this.makeMoveBtn.disabled = false;
    }

    highlightCell(cellElement) {
        // Remove previous highlights
        document.querySelectorAll('.cell.highlighted').forEach(el => {
            el.classList.remove('highlighted');
        });

        // Add highlight to selected cell
        cellElement.classList.add('highlighted');
    }

    clearAllSelections() {
        // Remove all highlights and selections
        document.querySelectorAll('.cell.highlighted, .cell.selected, .cell.destination-highlight').forEach(el => {
            el.classList.remove('highlighted', 'selected', 'destination-highlight');
            // Clear destination data attributes
            delete el.dataset.moveDirection;
            delete el.dataset.sourceRow;
            delete el.dataset.sourceCol;
        });

        // Hide all direction indicators
        document.querySelectorAll('.direction-indicators').forEach(indicator => {
            indicator.style.display = 'none';
        });

        // Remove selected direction styling
        document.querySelectorAll('.direction-arrow.selected-direction').forEach(arrow => {
            arrow.classList.remove('selected-direction');
        });
    }

    showLoading(message) {
        if (this.loadingElement) {
            this.loadingElement.textContent = message;
            this.loadingElement.style.display = 'block';
        }
        console.log(message);
    }

    hideLoading() {
        if (this.loadingElement) {
            this.loadingElement.style.display = 'none';
        }
    }

    showSuccess(message) {
        this.hideLoading();
        console.log(message);
        // You could implement a success notification here
    }

    showError(message) {
        this.hideLoading();
        console.error(message);
        // Show error in a non-intrusive way
        const errorDiv = document.createElement('div');
        errorDiv.className = 'error-message';
        errorDiv.textContent = message;
        errorDiv.style.cssText = `
            position: fixed;
            top: 20px;
            right: 20px;
            background: #dc3545;
            color: white;
            padding: 15px;
            border-radius: 8px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.3);
            z-index: 1000;
            max-width: 300px;
            font-size: 14px;
        `;
        document.body.appendChild(errorDiv);

        setTimeout(() => {
            if (errorDiv.parentNode) {
                errorDiv.parentNode.removeChild(errorDiv);
            }
        }, 5000);
    }
}

// Initialize the game when the page loads
document.addEventListener('DOMContentLoaded', () => {
    new QuixoGame();
});