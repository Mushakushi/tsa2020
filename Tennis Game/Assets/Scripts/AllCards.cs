using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RALLY CARDS HELP RALLY THE BALL (attack/defense/stat buff)
public delegate Vector3[] Path(Vector3 A, Vector3 point, Vector3 C, int capacity, bool isPlayer); //path that the ball will follow
public delegate void Effect(TennisPlayer player); //misc effect of card
public class Card
{
    public Path path; //path the ball will hit
    public float speedMultiplier; //how fast will the ball be hit?
    public Effect effect; //mis effect that ball wil have (if applicable)
    public int waitTime = 0; //how long have we waited for activation after cooldown? Cards reactivate once we have waited for cooldown 
    public int coolDown; //how long it takes until this card can be used again

    public Color color; //color of line made by this card 

    //Constructor for the card
    public Card(Path pathToFollow, float SpeedMultiplier, Effect miscEffect, int CoolDown, Color lineColor)
    {
        path = pathToFollow;
        speedMultiplier = SpeedMultiplier;
        effect = miscEffect; 
        coolDown = CoolDown;
        color = lineColor; 
    }
}

#region PATHS
public class Paths
{

    //BEZIER CURVE
    public static Vector3[] NormalBezier(Vector3 A, Vector3 point, Vector3 C, int capacity /*amount of points*/, bool isPlayer)
    {
        Vector3[] points = new Vector3[capacity];
        //CALCULATE POINT (VALUE OF T) TO CROSS POINT
        float a = Vector2.Distance(point, A);
        float b = Vector2.Distance(point, C);
        float k = a / (a + b);

        //CALCULATE POSITION OF CONTROL POINT TO CROSS POINT AT CALCULATED VALUE
        Vector3 B = (point - (1 - k) * (1 - k) * A - k * k * C) / (2 * (1 - k) * k);

        //DON'T EXTEND CONTROL POINT PAST TENNIS PLAYER/ AIM POSITION TO CREATE BACKWARDS BALL MOVEMENT
        if (isPlayer)
            B.z = Mathf.Clamp(B.z, A.z, C.z); //A -> Player -> C
        else
            B.z = Mathf.Clamp(B.z, C.z, A.z); //C <- Opponent <- A

        //CALCULATE GRAPH
        for (int i = 0; i <= capacity - 1; i++)
        {
            //GET VALUE OF T 
            float t = 1f / capacity * i;

            //GET POINTS AND ADD THE TO THE ARRAY
            points[i] = (1f - t) * (1f - t) * A + 2f * (1f - t) * t * B + t * t * C;

            //thanks to @Bunny83 and @SparrowsNest for all their help! 
            //https://answers.unity.com/questions/1700903/how-to-make-quadratic-bezier-curve-through-3-point.html?_ga=2.167879905.1985838484.1581976785-1145140738.1537626484
        }

        //RETURN PATHS
        return points; 
    }

    //LINEAR
    public static Vector3[] Linear(Vector3 A, Vector3 point, Vector3 C, int capacity, bool isPlayer)
    {
        Vector3[] points = new Vector3[capacity];

        //Go straight from point A to B
        for (int i = 0; i < capacity; i++)
        {
            points[i] = Vector3.Lerp(A, C, 1f / capacity * i); 
        }

        return points; 
    }
}
#endregion

#region EFFECTS
class Effects : MonoBehaviour
{
    public static void Jump(TennisPlayer player)
    {
        player.targetDirection.y += 2; 
    }
}
#endregion

//All Cards available in the game 
public class AllCards : MonoBehaviour
{
    //Attacking
    public Card normal_a = new Card(Paths.NormalBezier, 1f, null, 0, Color.blue);
    public Card jumpShot_a = new Card(Paths.Linear, 2f, Effects.Jump, 1, Color.red); 
}
