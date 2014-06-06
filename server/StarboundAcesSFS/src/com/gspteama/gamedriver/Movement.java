package com.gspteama.gamedriver;

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
    
    public void setPosition(float x, float y, float z){
        this.position = new float[]{x, y, z};
    }
    
    public void setRotation(float x, float y, float z, float w){
        this.rotation = new float[]{x, y, z, w};
    }
    
    public void setEulerRotation(float x, float y, float z){
        this.eulerRotation = new float[]{x, y, z};
    }
    
    public void onLeft(float value){
        
    }
    
    public void onRight(float value){
        
    }
    
    public void onUp(float value){
        
    }
    
    public void onDown(float value){
        
    }
    
    public void onHorizontal(float value){
        
    }
    
    public void onVertical(float value){
        
    }
    
    public void onManueverHorizontal(float value){
        
    }
    
    public void onManueverVertical(float value){
        
    }
    
    private float[] cross(float Ax, float Ay, float Az, float Bx, float By, float Bz){
        float result[] = new float[3];
        result[0] = (Ay*Bz) - (Az*By);
        result[1] = (Az*Bx) - (Ax*Bz);
        result[2] = (Ax*By) - (Ay*Bx);
        return result;
    }
}
