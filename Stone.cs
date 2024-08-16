namespace StoneSpace;

class Stone{

    public int x;
    public int y;
    public Stone parent;
    public int liberties;
    public string color;

    public Stone(int xCoord, int yCoord, string stoneColor){
        x = xCoord;
        y = yCoord;
        color = stoneColor;
    }

    public Stone(){}
}