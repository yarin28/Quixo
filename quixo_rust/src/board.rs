use std::fmt;

#[derive(Clone, Copy, PartialEq, Debug)]
pub enum Symbol {
    Empty,
    X,
    O,
}

#[derive(Clone, Copy, Debug)]
pub enum Direction {
    Up,
    Down,
    Left,
    Right,
}

pub struct Board {
    pub grid: [[Symbol; 5]; 5],
}

impl Board {
    pub fn new() -> Self {
        Board {
            grid: [[Symbol::Empty; 5]; 5],
        }
    }

    // Check if a position is on the periphery
    pub fn is_periphery(row: usize, col: usize) -> bool {
        row == 0 || row == 4 || col == 0 || col == 4
    }

    // Make a Quixo move: select a periphery cube, flip to symbol, and push in direction
    pub fn make_move(&mut self, row: usize, col: usize, direction: Direction, symbol: Symbol) -> Result<(), &'static str> {
        if !Self::is_periphery(row, col) {
            return Err("Must select from periphery");
        }

        // In Quixo, you can select any periphery piece (empty, your own, or opponent's)
        // The restriction is only on selecting opponent's pieces from non-periphery positions

        // Validate that the direction is valid for this position
        if !self.is_valid_direction(row, col, direction) {
            return Err("Invalid direction for this position");
        }

        // Push the row or column and place the piece at the end
        match direction {
            Direction::Up => self.push_column_up(col, symbol),
            Direction::Down => self.push_column_down(col, symbol),
            Direction::Left => self.push_row_left(row, symbol),
            Direction::Right => self.push_row_right(row, symbol),
        }
        Ok(())
    }

    fn is_valid_direction(&self, row: usize, col: usize, direction: Direction) -> bool {
        match direction {
            Direction::Up => row == 4, // Can only push up from bottom row
            Direction::Down => row == 0, // Can only push down from top row
            Direction::Left => col == 4, // Can only push left from right column
            Direction::Right => col == 0, // Can only push right from left column
        }
    }

    fn push_column_up(&mut self, col: usize, new_symbol: Symbol) {
        // Push column up: shift all pieces up, put new symbol at top end
        for i in 0..4 {
            self.grid[i][col] = self.grid[i + 1][col];
        }
        self.grid[0][col] = new_symbol;
    }

    fn push_column_down(&mut self, col: usize, new_symbol: Symbol) {
        // Push column down: shift all pieces down, put new symbol at bottom end
        for i in 0..4 {
            self.grid[i][col] = self.grid[i + 1][col];
        }
        self.grid[4][col] = new_symbol;
    }

    fn push_row_left(&mut self, row: usize, new_symbol: Symbol) {
        // Push row left: shift all pieces left, put new symbol at right end
        for i in 0..4 {
            self.grid[row][i] = self.grid[row][i + 1];
        }
        self.grid[row][4] = new_symbol;
    }

    fn push_row_right(&mut self, row: usize, new_symbol: Symbol) {
        // Push row right: shift all pieces right, put new symbol at right end
        for i in 0..4 {
            self.grid[row][i] = self.grid[row][i + 1];
        }
        self.grid[row][4] = new_symbol;
    }

    pub fn check_winner(&self) -> Option<Symbol> {
        // Check rows, columns, and diagonals for 5 in a row
        for i in 0..5 {
            if self.grid[i][0] != Symbol::Empty &&
               self.grid[i].iter().all(|&s| s == self.grid[i][0]) {
                return Some(self.grid[i][0]);
            }
            if self.grid[0][i] != Symbol::Empty &&
               (0..5).all(|j| self.grid[j][i] == self.grid[0][i]) {
                return Some(self.grid[0][i]);
            }
        }
        // Diagonals
        if self.grid[0][0] != Symbol::Empty &&
           (0..5).all(|i| self.grid[i][i] == self.grid[0][0]) {
            return Some(self.grid[0][0]);
        }
        if self.grid[0][4] != Symbol::Empty &&
           (0..5).all(|i| self.grid[i][4 - i] == self.grid[0][4]) {
            return Some(self.grid[0][4]);
        }
        None
    }
}

impl fmt::Display for Board {
    fn fmt(&self, f: &mut fmt::Formatter) -> fmt::Result {
        for row in &self.grid {
            for &cell in row {
                let symbol = match cell {
                    Symbol::Empty => ".",
                    Symbol::X => "X",
                    Symbol::O => "O",
                };
                write!(f, "{} ", symbol)?;
            }
            writeln!(f)?;
        }
        Ok(())
    }
}