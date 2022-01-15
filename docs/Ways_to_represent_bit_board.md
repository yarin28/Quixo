# ways to represent bit board -13.1.2022

## introduction

the bit board must have at least 50 bits $(5*5)*2$
the closest number that is a power of 2 is 64!  
the c# variable that has 64 bits is

~~~c#
unsigned long Pieces = 0;
~~~

but there is still the question how to represent the bit board
with the same $64bits$

## how to represent the board with long variable

there are 2 ways that i can represent the bit board-

1. have 2 sections of 25 bits for each player meaning  
    1.1 7 0 of un used bits.  
    1.2 25 bits of the x player.  
    1.3 7 bits of 0 unused.  
    1.4 25 of bits to the O player.  
$\stackrel{\text{unused}}{0000000}\stackrel{\text{X}}{000000000000000000000000}\stackrel{\text{unused}}{0000000}\stackrel{\text{Y}}{000000000000000000000000}$

2. by assigning each position on the board 2 bits to distinguish between X,O or zero  
2.1 if both of the bits are off then there is there  
2.2 if only the "small" one is on the X piece is there  
2.3 if only the "big/large" one is on the O piece is there  

to get to each piece we will have the make masks and shipt hem to the right
 position, and check more cases. Moreover all the moving of the pieces will be
harder

### what will probably be chosen

method 1
