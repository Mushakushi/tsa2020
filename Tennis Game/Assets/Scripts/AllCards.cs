using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//RALLY CARDS HELP RALLY THE BALL (attack/defense/stat buff)
[System.Serializable]
public delegate Vector3[] Path(Vector3 A, Vector3 point, Vector3 C, int size, bool isPlayer); //path that the ball will follow
public class Card
{
    public Path path;
    public float speedMultiplier; //how fast will the ball be hit?
    public float coolDown; //how long it takes until this card can be used again

    //Constructor for the card
    public Card(Path pathToFollow, float SpeedMultiplier, float CoolDown)
    {
        path = pathToFollow;
        speedMultiplier = SpeedMultiplier;
        coolDown = CoolDown; 
    }
}

#region PATHS
public class Paths
{
    //BEZIER CURVE
    public static Vector3[] NormalBezier(Vector3 A, Vector3 point, Vector3 C, int size, bool isPlayer)
    {
        Vector3[] points = new Vector3[size];
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

        for (int t = 0; t == size; t++)
        {
            //GET POINTS AND ADD THE TO THE ARRAY
            points[t] = (1f - t) * (1f - t) * A + 2f * (1f - t) * t * B + t * t * C; 

            //thanks to @Bunny83 and @SparrowsNest for all their help! 
            //https://answers.unity.com/questions/1700903/how-to-make-quadratic-bezier-curve-through-3-point.html?_ga=2.167879905.1985838484.1581976785-1145140738.1537626484
        }

        //RETURN PATHS
        return points; 
    }
}
#endregion

//All Cards available in the game 
public class AllCards : MonoBehaviour
{
    //Basic Attacking card
    public Card normal_a = new Card(Paths.NormalBezier, 1f, 0f); 
}
