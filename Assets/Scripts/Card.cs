using UnityEngine;

public class Card
{
    public enum Color{
        RED,
        BLUE,
        GREEN,
        YELLOW,
        BLACK
    }

    public enum Type{
        NUMBER,
        SKIP,
        REVERSE,
        WILD,
        DRAW2,
        DRAW4
    }


    private Color _color;
    private Type _type;
    private int _number;
    private Sprite _image;



    public Card(Color c, Type t, int n){
            _color = c;
            _type = t;
            _number = n;

            _image = Resources.Load<Sprite>(imagePath());
        }

    // public Card(Color c, Type t): this(c, t, -1){}
    public Card(Color c, Type t): this(c, t, (int)t * -1){}

    public Card(): this(Color.BLACK, Type.WILD){}


    public Color color{ get => _color; set => _color = value; }
    public Type type { get => _type; set => _type = value; }
    public int number { get => _number; set => _number = value; }
    public Sprite image { get => _image; set => _image = value; }

    public string imagePath(){

        string s = "Cards/";

        if(_color == Color.BLACK && _type == Type.WILD)
            return s + "Wild";
        if(_color == Color.BLACK && _type == Type.DRAW4)
            return s + "Wild_Draw";
        
        switch(_color){
            case Color.RED:
                s += "Red_";
                break;
            case Color.GREEN:
                s += "Green_";
                break;
            case Color.BLUE:
                s += "Blue_";
                break;
            case Color.YELLOW:
                s += "Yellow_";
                break;
        }

        switch(_type){
            case Type.NUMBER:
                return s + _number.ToString();
            case Type.DRAW2:
                return s + "Draw";
            case Type.REVERSE:
                return s + "Reverse";
            case Type.SKIP:
                return s + "Skip";
        }

        // Shouldnt reach here but just in case
        return s;
    }

    // Returns a string representing the card
    public string Stringify(){
        string s = "";

        if(color != Color.BLACK) s += color.ToString();

        if(type == Type.NUMBER) s += " " + number;
        else s += " " + type.ToString();

        return s + " card";
    }
}
