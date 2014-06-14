package com.gspteama.gamedriver;

import org.apache.commons.math3.geometry.euclidean.threed.Rotation;
import org.apache.commons.math3.geometry.euclidean.threed.RotationOrder;
import org.apache.commons.math3.geometry.euclidean.threed.Vector3D;
import org.lwjgl.util.vector.Matrix4f;
import org.lwjgl.util.vector.Vector3f;
import org.lwjgl.util.vector.Vector4f;

public class Movement{
    public float Velocity;
    public float Acceleration;
    
    private float maxVelocity;
    
    //protected Vector3D position = Vector3D.ZERO;
    protected Vector3f position = new Vector3f();
    protected Vector3f spawn = new Vector3f();
    //protected Vector3D rotation = Vector3D.ZERO;
    protected Vector3f rotation = new Vector3f();
    protected Vector4f quaternion = new Vector4f();
    
    //private static final Vector3D UP = Vector3D.PLUS_J;
    protected static final Vector3f UP = new Vector3f(0.0f, 1.0f, 0.0f);
    //private static final Vector3D LEFT = Vector3D.MINUS_I;
    protected static final Vector3f RIGHT = new Vector3f(1.0f, 0.0f, 0.0f);
    //private static final Vector3D FORWARD = Vector3D.PLUS_K;
    protected static final Vector3f FORWARD = new Vector3f(0.0f,0.0f, 1.0f);
    
    protected Matrix4f transform = new Matrix4f();
    
    public Vector3f toRadians(Vector3f in){
        return new Vector3f(
                in.getX() * (3.14f/180.0f),
                in.getY() * (3.14f/180.0f),
                in.getZ() * (3.14f/180.0f));
    }
    
    public void onUpdate(float deltaTime){
        Velocity = Acceleration;
        //Velocity = Velocity + Acceleration * deltaTime;
        if(Velocity >= maxVelocity){
            Velocity = maxVelocity;
        }
        
        Rotation rotator = new Rotation(RotationOrder.XYZ, rotation.getX(),
                rotation.getY(),
                rotation.getZ());
        quaternion = new Vector4f((float)rotator.getQ0(), 
                (float)rotator.getQ1(),
                (float)rotator.getQ2(), 
                (float)rotator.getQ3());
        
        transform = new Matrix4f();
        
        Matrix4f.scale(new Vector3f(1.0f, 1.0f, 1.0f), transform, transform);
        Matrix4f.translate(position, transform, transform);
        Matrix4f.rotate(toRadians(rotation).getZ(), FORWARD, transform, transform);
        Matrix4f.rotate(toRadians(rotation).getY(), UP, transform, transform);
        Matrix4f.rotate(toRadians(rotation).getX(), RIGHT, transform, transform);
        
        //to row major
        transform.transpose();
        
        //get the forward vector
        Vector3f forward = new Vector3f(transform.m20,
                                        transform.m21,
                                        transform.m22);
        forward.normalise();
                                        
        Vector3f forwardVel = new Vector3f(forward.getX() * Velocity * deltaTime,
                                            forward.getY() * Velocity * deltaTime,
                                            forward.getZ() * Velocity * deltaTime);
        
        Vector3f.add(position, forwardVel, position);
    }
    
    public float getMaxVelocity(){
        return maxVelocity;
    }

    public Movement(float maxVelocity){
        this.maxVelocity = maxVelocity;
        Velocity = 0.0f;
        Acceleration = 0.0f;
        transform.setIdentity();
    }
    
    public void setSpawn(float x, float y, float z){
        spawn.set(x, y, z);
        position.set(x, y, z);
    }
    
    public static float distance(float ax, float ay, float az,
            float bx, float by, float bz){
        return Vector3f.dot(
                new Vector3f(ax, ay, az),
                new Vector3f(bx, by, bz)
        );
    }
    
    public float[] getSpawn(){
        return new float[]{spawn.getX(), spawn.getY(), spawn.getZ()};
    }
    
    public void onLeft(float value){//A
        rotation.y += value;
    }
    
    public void onRight(float value){//D
        onLeft(value);
    }
    
    public void onUp(float value){//W
        Acceleration += value;
    }
    
    public void onDown(float value){//S
        Acceleration -= value;
    }
    
    public void onHorizontal(float value){//MouseX
        rotation.x += value;
    }
    
    public void onVertical(float value){//MouseY
        rotation.z += value;
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
    
    public float[] getPosition(){
        return new float[]{position.getX(),
            position.getY(),
            position.getZ()
        };
    }
    
    public float[] getQuaternion(){
        return new float[]{quaternion.getX(),
            quaternion.getY(),
            quaternion.getZ(),
            quaternion.getW()
        };
    }
}
