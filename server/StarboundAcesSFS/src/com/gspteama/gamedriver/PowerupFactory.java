package com.gspteama.gamedriver.factory;

public class PowerupFactory{
    public static IPowerup getPowerup(String fullyQualifiedName){
        return (IPowerup)Class.forName(fullyQualifiedName).newInstance();
    }
}
