package com.gspteama.gamedriver

public class Movement{
    public float Velocity;
    public float Acceleration;
    private float maxVelocity;
    
    private float position[];
    private float rotation[];
    private float eulerRotation[];

    public Movement(float maxVelocity){
      this.maxVelocity = maxVelocity;
      position = new float[3];
      rotation = new float[4];
      eulerRotation = new float[3];
      Velocity = 0.0f;
      Acceleration = 0.0f;
    }

    public float[] getPosition() {
        return position;
    }

    public void setPosition(float[] position) {
        this.position = position;
    }

    public float[] getRotation() {
        return rotation;
    }
    
    public float[] getEulerRotation(){
        return eulerRotation;
    }

    public void setRotation(float[] rotation) {
        this.rotation = rotation;
    }
    
    public void setEulerRotation(float[] eulerRotation){
        this.eulerRotation = eulerRotation;
    }
    
    public float setPosition(float x, float y, float z){
        this.position = new float[]{x, y, z};
    }
    
    public float setRotation(float x, float y, float z, float w){
        this.rotation = new float[]{x, y, z, w};
    }
    
    public float setEulerRotation(float x, float y, float z){
        this.eulerRotation = new float[]{x, y, z};
    }
}
