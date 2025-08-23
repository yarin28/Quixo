use crate::board::{Symbol, Direction};

#[derive(Clone, Copy)]
pub struct Player {
    pub symbol: Symbol,
    pub name: &'static str,
}

impl Player {
    pub fn new(symbol: Symbol, name: &'static str) -> Self {
        Player { symbol, name }
    }
}

pub struct Move {
    pub row: usize,
    pub col: usize,
    pub direction: Direction,
}

impl Move {
    pub fn new(row: usize, col: usize, direction: Direction) -> Self {
        Move { row, col, direction }
    }
}