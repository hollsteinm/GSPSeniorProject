package com.gspteama.gamedriver;

import org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
import org.apache.commons.math3.geometry.euclidean.threed.Rotation;

public class Movement{
    public float Velocity;
    public float Acceleration;
    
    private float maxVelocity;
    
    protected Vector3D position = Vector3D.ZERO;
    protected Vector3D rotation = Vector3D.ZERO;
    private Rotation rotator = Rotation.IDENTITY;
    
    private static final Vector3D UP = Vector3D.PLUS_J;
    private static final Vector3D LEFT = Vector3D.MINUS_I;
    private static final Vector3D FORWARD = Vector3D.PLUS_K;
    
    public void onUpdate(float deltaTime){
        Velocity = Velocity + Acceleration * deltaTime;
        position = position.add(rotation.normalize()).scalarMultiply(Velocity);
    }

    public Movement(float maxVelocity){
        this.maxVelocity = maxVelocity;
        Velocity = 0.0f;
        Acceleration = 0.0f;
    }
    
    public void onLeft(float value){//A
        rotation = rotation.add(UP.scalarMultiply(value));
    }
    
    public void onRight(float value){//D
        rotation = rotation.add(UP.scalarMultiply(value));
    }
    
    public void onUp(float value){//W
        Acceleration += value;
    }
    
    public void onDown(float value){//S
        Acceleration -= value;
    }
    
    public void onHorizontal(float value){//MouseX
        rotation = rotation.add(FORWARD.scalarMultiply(value));
    }
    
    public void onVertical(float value){//MouseY
        rotation = rotation.add(LEFT.scalarMultiply(value));
    }
    
    //I am torn here, the manuevers will take alot to replicate from the C# code
    //lacking any meaningful game style Transfrom libraries akin to that of Unity
    //Kinematics were easy. But this may take some time to implement as it is involved
    //would require setting a player state and blocking input then allowing input after
    //the player is done manuvers.
    //Complexities:
    //BarrelRoll = easy
    //LoopFull = easy
    //LoopWithFlip = medium
    //The issue is how involved the sync will be with movement and player state.
    public void onManueverHorizontal(float value){//B(1)+W || S
        //just propojate to clients?
        //then update position..?
    }
    
    public void onManueverVertical(float value){//B(1)+A || D
        //just propojate to clients?
        //then update position..?
    }
}
