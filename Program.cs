﻿using System;
using System.IO;
using StoneSpace;

class GoGame{
    private int BOARD_SIZE;
    private bool GAME_WON = false;
    private string playerTurn = "black";
    private bool lastTurnPass = false;
    Stone[,] stones;
    HashSet<Stone> visited = new HashSet<Stone>();
    HashSet<(int, int)> liberties = new HashSet<(int, int)>(); 
    HashSet<(int, int)> visitedSpaces = new HashSet<(int, int)>();


    public GoGame(int boardSize){
        BOARD_SIZE = boardSize;
        stones = new Stone[boardSize, boardSize];
    }


    private void fillRegion(int y, int x, string color){
        if(visitedSpaces.Contains((y, x))){
            return;
        }
        visitedSpaces.Add((y, x));
        try{
            if(stones[y, x-1]==null){
                fillRegion(y, x-1, color);
            }
            else if(stones[y, x-1].color==color){
                fillRegion(y, x-1, color);
            }
        }catch(Exception e){}
        try{
            if(stones[y-1, x]==null){
                fillRegion(y-1, x, color);
            }
            else if(stones[y-1, x].color==color){
                fillRegion(y-1, x, color);
            }
        }catch(Exception e){}
        try{
            if(stones[y, x+1]==null){
                fillRegion(y, x+1, color);
            }
            else if(stones[y, x+1].color==color){
                fillRegion(y, x+1, color);
            }
        }catch(Exception e){}
        try{
            if(stones[y+1, x]==null){
                fillRegion(y+1, x, color);
            }
            else if(stones[y+1, x].color==color){
                fillRegion(y+1, x, color);
            }
        }catch(Exception e){}
    }



    private int rawScores(string color){
        for(int i = 0; i<BOARD_SIZE; i++){
            for(int j = 0; j<BOARD_SIZE; j++){
                try{
                    if(stones[i, j].color==color){
                        fillRegion(i, j, color);
                    }
                }catch(Exception e){}
            }
        }

        int score = visitedSpaces.Count;
        visitedSpaces.Clear();
        return score;
    }


    private void finishGame(){
        float blackScore = (float)rawScores("black");
        float whiteScore = (float)rawScores("white");

        if(blackScore + whiteScore > BOARD_SIZE*BOARD_SIZE){
            float neutralSpaces = ((whiteScore+blackScore)-BOARD_SIZE*BOARD_SIZE);
            blackScore -= neutralSpaces;
            whiteScore -= neutralSpaces;
        }

        whiteScore += (float)6.5;

        if(blackScore>whiteScore){
            Console.WriteLine("Black wins!");
        }else{
            Console.WriteLine("White wins!");
        }
        Console.WriteLine("Black score:" + blackScore);
        Console.WriteLine("White score:" + whiteScore);
        Environment.Exit(0);
    }


    private int countLiberties(Stone s){
        getLiberties(s);
        int libertiesCount = liberties.Count;
        if(libertiesCount==0){
            deleteSet(s);
        }

        foreach(Stone i in visited){
            i.liberties = libertiesCount;
        }

        visited.Clear();
        liberties.Clear();

        return libertiesCount;
    }

    
    private void getLiberties(Stone s){
        visited.Add(s);
        try{
            if(stones[s.y-1, s.x]==null & !liberties.Contains((s.y-1, s.x))){
                liberties.Add((s.y-1, s.x));
            }
            else if(stones[s.y, s.x].color==stones[s.y-1, s.x].color & !visited.Contains(stones[s.y-1, s.x])){
                getLiberties(stones[s.y-1, s.x]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y, s.x-1]==null & !liberties.Contains((s.y, s.x-1))){
                liberties.Add((s.y, s.x-1));
            }
            else if(stones[s.y, s.x].color==stones[s.y, s.x-1].color & !visited.Contains(stones[s.y, s.x-1])){
                getLiberties(stones[s.y, s.x-1]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y+1, s.x]==null & !liberties.Contains((s.y+1, s.x))){
                liberties.Add((s.y+1, s.x));
            }
            else if(stones[s.y, s.x].color==stones[s.y+1, s.x].color & !visited.Contains(stones[s.y+1, s.x])){
                getLiberties(stones[s.y+1, s.x]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y, s.x+1]==null & !liberties.Contains((s.y, s.x+1))){
                liberties.Add((s.y, s.x+1));
            }
            else if(stones[s.y, s.x].color==stones[s.y, s.x+1].color & !visited.Contains(stones[s.y, s.x+1])){
                getLiberties(stones[s.y, s.x+1]);
            }
        }catch(Exception e){}
        
    }
    

    private void deleteSet(Stone s){
        gatherSet(s);
        foreach(Stone i in visited){
            stones[i.y, i.x] = null;
        }
        visited.Clear();
    }

    
    private void gatherSet(Stone s){
        visited.Add(s);
        try{
            if(stones[s.y, s.x].color==stones[s.y-1, s.x].color & !visited.Contains(stones[s.y-1, s.x])){
                gatherSet(stones[s.y-1, s.x]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y, s.x].color==stones[s.y, s.x-1].color & !visited.Contains(stones[s.y, s.x-1])){
                gatherSet(stones[s.y, s.x-1]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y, s.x].color==stones[s.y+1, s.x].color & !visited.Contains(stones[s.y+1, s.x])){
                gatherSet(stones[s.y+1, s.x]);
            }
        }catch(Exception e){}
        try{
            if(stones[s.y, s.x].color==stones[s.y, s.x+1].color & !visited.Contains(stones[s.y, s.x+1])){
                gatherSet(stones[s.y, s.x+1]);
            }
        }catch(Exception e){}
    }
    


    private void placeStone(int x, int y){
        stones[y, x] = new Stone(x, y, playerTurn);
        try{
            if(stones[y, x].color!=stones[y-1, x].color){
                countLiberties(stones[y-1, x]);
            }
        }catch(Exception e){}
        try{
            if(stones[y, x].color!=stones[y, x-1].color){
                countLiberties(stones[y, x-1]);
            }
        }catch(Exception e){}
        try{
            if(stones[y, x].color!=stones[y+1, x].color){
                countLiberties(stones[y+1, x]);
            }
        }catch(Exception e){}
        try{
            if(stones[y, x].color!=stones[y, x+1].color){
                countLiberties(stones[y, x+1]);
            }
        }catch(Exception e){}

        countLiberties(stones[y, x]);
    }


    private bool validPlacement(int xCoord, int yCoord){
        if(yCoord>=0 & yCoord<=BOARD_SIZE-1 & xCoord>=0 & xCoord<=BOARD_SIZE-1){
            return true;
        }
        else{
            return false;
        }
    }


    private void handleInput(string input){
        string[] command = input.Split(" ");

        if(command[0]=="place"){
            lastTurnPass = false;
            try{
                if(validPlacement(int.Parse(command[1]), int.Parse(command[2]))==false){
                    Console.WriteLine("invalid placement, try again: ");
                    string innerInput = Console.ReadLine();
                    handleInput(innerInput);
                    return;
                }
                else{
                    placeStone(int.Parse(command[1]), int.Parse(command[2]));
                }
            }catch(Exception e){
                Console.WriteLine("placement must be in the format: x y");
            }
        }
        else if(command[0]=="pass"){
            if(lastTurnPass==true){
                GAME_WON = true;
                finishGame();
            }
            lastTurnPass = true;
            Console.WriteLine(playerTurn + " has passed this turn.");
            return;
        }
        else if(command[0]=="resign"){
            lastTurnPass = false;
            GAME_WON = true;
            if(playerTurn=="white"){
                Console.WriteLine("black wins!");
            }
            else if(playerTurn=="black"){
                Console.WriteLine("white wins!");
            }
            Environment.Exit(0);
        }
    }


    private void printBoard(){
        Console.Write("\n\n\n");
        Console.Write("  ");
        for(int i = 0; i<BOARD_SIZE; i++){
            Console.Write(i + "   ");
        }
        Console.WriteLine();
        
        
        for(int i = 0; i<BOARD_SIZE; i++){
            Console.Write(i + " ");

            for(int j = 0; j<BOARD_SIZE; j++){
                
                if(stones[i, j]==null){
                    Console.Write(" ");
                }
                else if(stones[i, j].color=="black"){
                    Console.Write("X");
                }
                else if(stones[i,j].color=="white"){
                    Console.Write("O");
                }
                if(j!=BOARD_SIZE-1){
                    Console.Write("---");
                }
                 
            }
            
            Console.WriteLine();
            Console.Write("  ");
            if(i!=BOARD_SIZE-1){
                for(int k = 0; k<BOARD_SIZE-1; k++){
                    Console.Write("|   ");
                }
                Console.WriteLine("|");
            }
        }
    }

    
    private void togglePlayerTurn(){
        if(playerTurn=="black"){
            playerTurn = "white";
        }
        else if(playerTurn=="white"){
            playerTurn = "black";
        }
    }


    public void gameLoop(){
        while(GAME_WON==false){
            printBoard();
            string input = Console.ReadLine();
            handleInput(input);
            togglePlayerTurn();
        }
    }


    static void Main(String[] args){
        
        if(int.Parse(args[0])>4 & int.Parse(args[0])<20){
            var GG = new GoGame(int.Parse(args[0]));
            GG.gameLoop();
        }
        else{
            Console.WriteLine("When running this program, give a board size 5-19 as an argument.");
            Environment.Exit(0);
        }
        
    }
}