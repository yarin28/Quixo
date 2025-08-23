mod board;
mod player;

use board::{Board, Symbol, Direction};
use player::{Player, Move};
use axum::{
    extract::State,
    http::StatusCode,
    response::Json,
    routing::{get, post},
    Router,
};
use tower_http::services::ServeDir;
use serde::{Deserialize, Serialize};
use std::sync::{Arc, Mutex};
use tokio::time::{timeout, Duration};

#[derive(Clone)]
struct AppState {
    game: Arc<Mutex<Game>>,
}

struct Game {
    board: Board,
    player1: Player,
    player2: Player,
    current_player: Player,
    winner: Option<Symbol>,
}

impl Game {
    fn new() -> Self {
        Game {
            board: Board::new(),
            player1: Player::new(Symbol::X, "Player 1"),
            player2: Player::new(Symbol::O, "Player 2"),
            current_player: Player::new(Symbol::X, "Player 1"),
            winner: None,
        }
    }
}

#[derive(Deserialize)]
struct MakeMoveRequest {
    row: usize,
    col: usize,
    direction: String,
}

#[derive(Serialize)]
struct GameStatus {
    board: String,
    current_player: String,
    winner: Option<String>,
}

#[derive(Serialize)]
struct PossibleMove {
    row: usize,
    col: usize,
    direction: String,
}

#[derive(Serialize)]
struct PossibleMovesResponse {
    moves: Vec<PossibleMove>,
}

async fn start_game(State(state): State<AppState>) -> Json<GameStatus> {
    let mut game = state.game.lock().unwrap();
    *game = Game::new();
    get_status(&game)
}

async fn make_move(
    State(state): State<AppState>,
    Json(req): Json<MakeMoveRequest>,
) -> Result<Json<GameStatus>, StatusCode> {
    let direction = match req.direction.as_str() {
        "up" => Direction::Up,
        "down" => Direction::Down,
        "left" => Direction::Left,
        "right" => Direction::Right,
        _ => return Err(StatusCode::BAD_REQUEST),
    };

    // Timeout for the move (30 seconds)
    let result = timeout(Duration::from_secs(30), async {
        let mut game = state.game.lock().unwrap();
        if game.winner.is_some() {
            return Err(StatusCode::BAD_REQUEST);
        }

        let current_symbol = game.current_player.symbol;
        let mv = Move::new(req.row, req.col, direction);
        if let Err(_) = game.board.make_move(mv.row, mv.col, mv.direction, current_symbol) {
            return Err(StatusCode::BAD_REQUEST);
        }

        game.winner = game.board.check_winner();
        if game.winner.is_none() {
            game.current_player = if current_symbol == Symbol::X {
                game.player2
            } else {
                game.player1
            };
        }

        Ok(get_status(&game))
    }).await;

    match result {
        Ok(res) => res,
        Err(_) => Err(StatusCode::REQUEST_TIMEOUT), // Timeout occurred
    }
}

async fn get_game_status(State(state): State<AppState>) -> Json<GameStatus> {
    let game = state.game.lock().unwrap();
    get_status(&game)
}

async fn get_possible_moves(State(state): State<AppState>) -> Json<PossibleMovesResponse> {
    let game = state.game.lock().unwrap();
    let mut moves = Vec::new();

    if game.winner.is_some() {
        return Json(PossibleMovesResponse { moves });
    }

    // Generate possible moves from periphery positions with correct direction logic
    for row in 0..5 {
        for col in 0..5 {
            if Board::is_periphery(row, col) {
                // In Quixo, you can select any periphery piece (empty, your own, or opponent's)
                // The move logic will handle replacing whatever is at the destination
                let can_select = Board::is_periphery(row, col);

                if can_select {
                    // Determine valid directions based on position
                    let valid_directions = get_valid_directions(row, col);

                    for direction_str in valid_directions {
                        moves.push(PossibleMove {
                            row,
                            col,
                            direction: direction_str.to_string(),
                        });
                    }
                }
            }
        }
    }

    Json(PossibleMovesResponse { moves })
}

fn get_valid_directions(row: usize, col: usize) -> Vec<&'static str> {
    let mut directions = Vec::new();

    // Top edge (row 0) - can only push down (toward center)
    if row == 0 && col != 0 && col != 4 {
        directions.push("down");
    }
    // Bottom edge (row 4) - can only push up (toward center)
    else if row == 4 && col != 0 && col != 4 {
        directions.push("up");
    }
    // Left edge (col 0) - can only push right (toward center)
    else if col == 0 && row != 0 && row != 4 {
        directions.push("right");
    }
    // Right edge (col 4) - can only push left (toward center)
    else if col == 4 && row != 0 && row != 4 {
        directions.push("left");
    }
    // Top-left corner (0,0) - can push down or right
    else if row == 0 && col == 0 {
        directions.push("down");
        directions.push("right");
    }
    // Top-right corner (0,4) - can push down or left
    else if row == 0 && col == 4 {
        directions.push("down");
        directions.push("left");
    }
    // Bottom-left corner (4,0) - can push up or right
    else if row == 4 && col == 0 {
        directions.push("up");
        directions.push("right");
    }
    // Bottom-right corner (4,4) - can push up or left
    else if row == 4 && col == 4 {
        directions.push("up");
        directions.push("left");
    }

    directions
}

fn get_status(game: &Game) -> Json<GameStatus> {
    Json(GameStatus {
        board: format!("{}", game.board),
        current_player: game.current_player.name.to_string(),
        winner: game.winner.map(|s| format!("{:?}", s)),
    })
}

#[tokio::main]
async fn main() {
    let game = Arc::new(Mutex::new(Game::new()));
    let app_state = AppState { game };

    let app = Router::new()
        .route("/start", post(start_game))
        .route("/move", post(make_move))
        .route("/status", get(get_game_status))
        .route("/possible-moves", get(get_possible_moves))
        .nest_service("/", ServeDir::new("static"))
        .with_state(app_state);

    let listener = tokio::net::TcpListener::bind("0.0.0.0:3000").await.unwrap();
    println!("Server running on http://0.0.0.0:3000");
    println!("Press Ctrl+C to stop the server");

    axum::serve(listener, app)
        .with_graceful_shutdown(async {
            tokio::signal::ctrl_c()
                .await
                .expect("Failed to listen for shutdown signal");
            println!("Shutting down server...");
        })
        .await
        .unwrap();
}
