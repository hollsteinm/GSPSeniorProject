/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public class Ship {
    private float       position[]  = new float[]{0.0f, 0.0f, 0.0f};
    private float       rotation[]  = new float[]{0.0f, 0.0f, 0.0f, 0.0f};

    public float[] getPosition() {
        return position;
    }

    public void setPosition(float[] position) {
        this.position = position;
    }

    public float[] getRotation() {
        return rotation;
    }

    public void setRotation(float[] rotation) {
        this.rotation = rotation;
    }
    
}
