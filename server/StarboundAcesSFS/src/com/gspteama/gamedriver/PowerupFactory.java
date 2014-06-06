package com.gspteama.gamedriver;

public class PowerupFactory{
    public static IPowerup getPowerup(String fullyQualifiedName)throws ClassNotFoundException, InstantiationException, IllegalAccessException{
        return (IPowerup)Class.forName(fullyQualifiedName).newInstance();
    }
}
