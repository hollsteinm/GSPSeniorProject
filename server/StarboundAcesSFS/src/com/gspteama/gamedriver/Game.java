/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.gamedriver;

import java.util.ArrayList;
import java.util.HashMap;

/**
 *
 * @author Martin
 */
public class Game implements IEventMessenger, Runnable{
    private enum EGameState{
        QUEUING,
        ACTIVE,
        ENDED,
    }
    
    
    private HashMap<Integer, Player>        players             = new HashMap<>();
    private HashMap<Integer, Projectile>    firedProjectiles    = new HashMap<>();
    private EGameState                      state               = EGameState.QUEUING;
    
    private static final ArrayList<String> allowedCommands = new ArrayList<>();
    
    private long lastTime;
    
    private long deltaTime(){
        long time = System.nanoTime();
        long delta = time - lastTime;
        lastTime = time;
        return delta;
    }
    
    public Game(){
        allowedCommands.add("left");
        allowedCommands.add("right");
        allowedCommands.add("up");
        allowedCommands.add("down");
        allowedCommands.add("fire1");
        allowedCommands.add("fire2");
        allowedCommands.add("reload");
        allowedCommands.add("vertical");
        allowedCommands.add("horizontal");
        allowedCommands.add("manuever_vertical");
        allowedCommands.add("manuever_horizontal");
    }
    
    public void inititialize(){
        lastTime = System.nanoTime();
    }

    public void AddPlayer(Player player){
        if(!players.containsKey((int)player.getPlayerId())){
            players.put((int)player.getPlayerId(), player);
        }
    }
    
    public void registerCollisionEvent(Player requester, Player other){
        
    }
    
    public void registerCollisionEvent(Player requester, Projectile other){
        
    }
    
    public void registerCollisionEvent(Player requester, IPowerup other){
        
    }
    
    public void registerCollisionEvent(Player requester, Obstacle Other){        
        
        
    }
    
    public void registerInputEvent(Player requester, String command, float value) throws Exception{
        if(allowedCommands.contains(command)){
            switch(command){
                case "left":
                    requester.getShip().movement.onLeft(value);
                    break;
                    
                case "right":
                    requester.getShip().movement.onRight(value);
                    break;
                    
                case "up":
                    requester.getShip().movement.onUp(value);
                    break;
                    
                case "down":
                    requester.getShip().movement.onDown(value);
                    break;
                    
                case "horizontal":
                    requester.getShip().movement.onHorizontal(value);
                    break;
                    
                case "vertical":
                    requester.getShip().movement.onVertical(value);
                    break;
                    
                case "fire1":
                    break;
                    
                case "fire2":
                    break;
                    
                case "reload":
                    requester.getShip().getWeapon().onReload();
                    break;
                    
                case "manuever_vertical":
                    requester.getShip().movement.onManueverVertical(value);
                    break;
                    
                case "manuever_horizontal":
                    requester.getShip().movement.onManueverHorizontal(value);
                    break;
                    
                default:
                    throw new Exception("Invalid command snuck through: " + command);
            }
            
        } else {
            throw new Exception("Invalid command: " + command);
        }
    }
    public void registerShootEvent(Player requester) throws Exception{
        if(requester.getShip().getWeapon().canFire()){
            Projectile p = requester.getShip().getWeapon().onFire();
            OnEvent("projectile.spawn", p);
            this.addProjectile(p.hashCode(), p);
        }        
    }
    
    public Player getPlayer(int playerId){
        return players.get(playerId);
    }
    
    //adds projecitle to game, returns its key in hashmap
    public void addProjectile(int id, Projectile p){
        firedProjectiles.put(id, p);
    }
    
    public Projectile getProjectile(int id){
        return firedProjectiles.get(id);
    }

    private ArrayList<IEventListener> listeners = new ArrayList<IEventListener>();
    
    @Override
    public void Register(IEventListener listener) {
        listeners.add(listener);
    }

    @Override
    public void Unregister(IEventListener listener) {
        listeners.remove(listener);
    }

    @Override
    public void OnEvent(String event, Object data) {
        for(IEventListener l : listeners){
            l.Notify(event, data);
        }
    }
    
    private void update(float deltaTime){
        for(Player p : players.values()){
            p.update(deltaTime);
            HashMap<String, Object> data = new HashMap<String, Object>();
            
            Movement shipMovement = p.getShip().movement;
            data.put("player.position", shipMovement.getPosition());            
            data.put("player.rotation", shipMovement.getQuaternion());            
            data.put("player.id", new Long(p.getPlayerId()));            
            OnEvent("player.update", data);
        }
        
        for(Projectile p : firedProjectiles.values()){
            p.update(deltaTime);
            HashMap<String, Object> data = new HashMap<String, Object>();
            
            Movement pMovement = p.movement;
            data.put("projectile.position", pMovement.getPosition());
            data.put("projectile.rotation", pMovement.getQuaternion());
            data.put("projectile.id", new Long(p.getProjectileID()));
            OnEvent("projectile.update", new HashMap<Integer, Float>());
        }
        //send new transforms and collision confirmations;
        
        
        //clear collision handshakes
    }

    @Override
    public void run() {
        update(deltaTime());
    }
    
    public boolean isPlaying(){
        return state == EGameState.ACTIVE;
    }
    
    public boolean isQueuing(){
        return state == EGameState.QUEUING;
    }
    
    public boolean isOver(){
        return state == EGameState.ENDED;
    }
}
