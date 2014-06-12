/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashMap;

/**
 *
 * @author Martin
 * 
 * //Add to database
 * table sa_powerup
 * * sequence sa_powerup_id_seq references powerup_id
 * * powerup_id : bigint unique not null default::sequence next val
 * * powerup_effect_class_name : charvar(128) unique not null
 * * powerup_effect_short_name : charvar(32) unique not null
 * 
 * table sa_hull
 * * sequence sa_hull_id references hull_id
 * * hull_id : bigint unique not null default::sequence next val
 * * hull_max_health : bigint not null
 * 
 * table sa_ship
 * * sequence sa_ship_id references ship_id
 * * ship_id : bigint unqiue not null default::sequence next val
 * * ship_hull_id : bigint not null (references hull_id)
 * * ship_energy : bigint not null
 * * ship_velocity : bigint not null
 * * ship_name : charvar(128) not null unique
 */
public class DBService {
    
    
    public static HashMap<String, Object> selectShipConfiguration(Connection connection, long shipId) throws Exception{
        HashMap<String, Object> results = new HashMap<>();
        Connection con = connection;
        PreparedStatement ps;
        ResultSet rs;
        
        String stmt = "select * from sa_ship join sa_hull on hull_id = ship_hull_id where ship_id = ? ";
        
        ps = con.prepareStatement(stmt);
        ps.setLong(1, shipId);
        
        rs = ps.executeQuery();
        
        if(rs.next()){
            results.put("hull", new com.gspteama.gamedriver.Hull(rs.getLong("hull_max_health")));
            results.put("name", rs.getString("ship_name"));
            results.put("maxVelocity", rs.getLong("ship_velocity"));
            results.put("maxEngergy", rs.getLong("ship_energy"));
        } else {
            rs.close();
            ps.close();
            con.close();
            throw new Exception("Ship not found. ID: " + shipId);
        }
        
        rs.close();
        ps.close();
        con.close();
        return results;
    }
    
    public static com.gspteama.gamedriver.IPowerup selectPowerup(Connection connection, String powerupShortName) throws Exception{
        Connection con = connection;
        PreparedStatement ps;
        ResultSet rs;
        
        com.gspteama.gamedriver.IPowerup powerup = null;
        
        String stmt = "select powerup_effect_class_name from sa_powerup where powerup_effect_short_name = ?";
        
        ps = con.prepareStatement(stmt);
        ps.setString(1, powerupShortName);
        
        rs = ps.executeQuery();
        
        if(rs.next()){
            powerup = com.gspteama.gamedriver.PowerupFactory.getPowerup(
                    rs.getString("powerup_effect_class_name")
                );
                
            ps.close();
            rs.close();
            con.close();
            return powerup;
        } else {
            ps.close();
            rs.close();
            con.close();
            throw new Exception("Powerup not found. Short Name: " + powerupShortName);
        }
        
    }
    
    public static com.gspteama.gamedriver.Weapon selectWeaponConfigurations(Connection connection, String weaponName, long owningPlayerId) throws Exception{
        Connection con = connection;
        PreparedStatement ps;
        ResultSet rs;
        com.gspteama.gamedriver.Weapon result = null;
        
        String stmt = "select * from sa_weapon_config swc " +
                " join sa_ammo_config sac " + 
                " on sac.ammo_config_id = swc.weapon_config_ammo_id " +
                " where swc.weapon_config_name = ? ";
        
        ps = con.prepareStatement(stmt);
        ps.setString(1, weaponName);
        
        rs = ps.executeQuery();
        
        if(rs.next()){
            
            result = new com.gspteama.gamedriver.Weapon(
                            rs.getFloat("weapon_config_cooldown"),
                            new com.gspteama.gamedriver.Projectile(
                                    owningPlayerId,
                                    rs.getLong("ammo_config_id"),
                                    rs.getString("ammo_config_name"),
                                    rs.getFloat("ammo_config_damage"),
                                    rs.getFloat("sa_ammo_speed"),
                                    rs.getFloat("ammo_config_range")),
                                rs.getInt("weapon_config_max_clip_size"),
                                rs.getInt("weapon_config_total_ammo")
                                
                        ); 
                        
            rs.close();
            ps.close();
            con.close();
            return result;
                        
        } else {
            rs.close();
            ps.close();
            con.close();
            throw new Exception("Weapon Configuration not found. Weapon Name: " + weaponName);
            
        }     
    }
    
    public static com.gspteama.gamedriver.Projectile selectProjectile(Connection connection, String projectileName, long owningPlayerId) throws Exception{
        Connection con = connection;
        PreparedStatement ps;
        ResultSet rs;
        
        String stmt = "select * from sa_ammo_config where ammo_config_name = ?";
        
        ps = con.prepareStatement(stmt);
        ps.setString(1, projectileName);
        
        rs = ps.executeQuery();
        com.gspteama.gamedriver.Projectile p = null;
        
        if(rs.next()){
           p = new com.gspteama.gamedriver.Projectile(
                   owningPlayerId,
                   rs.getLong("ammo_config_id"),
                   rs.getString("ammo_config_name"),
                   rs.getFloat("ammo_config_damage"),
                   rs.getFloat("sa_ammo_speed"),
                   rs.getFloat("ammo_config_range")
           );
           
            rs.close();
            ps.close();
            con.close();
            return p;
        } else {
            rs.close();
            ps.close();
            con.close();
            throw new Exception("Projectile not found. Projectile Name: " + projectileName);
        }
        
        
    }
    
    public static void insertNewGame(Connection connection, String gameName, int creatingUserId) throws SQLException{
        Connection con = connection;
        PreparedStatement ps = null;
        
        String stmtinsert = "insert into sa_game (game_name, game_state, game_user_list) values (?, 'Q', '{ 0,0 }');";
        
        con.setAutoCommit(false);
        ps = con.prepareStatement(stmtinsert);
        ps.setString(1, gameName);
        ps.executeUpdate();
        
        ps.close();
        con.commit();
        
        con.close();          
    }
    
    public static ArrayList<String> getQueuedGames(Connection connection) throws SQLException{
        ArrayList<String> results = new ArrayList<>();
        Connection con = connection;
        PreparedStatement ps;
        ResultSet rs;
        
        String selectstmt = "select game_name from sa_game where game_state = 'Q';";
        
        ps = con.prepareStatement(selectstmt);
        rs = ps.executeQuery();
        
        while(rs.next()){
            results.add(rs.getString("game_name"));
        }
        
        rs.close();
        ps.close();
        con.close();        
        
        return results;
    }
    
    public static void updateGameStatus(Connection connection, String newState, String gameName) throws SQLException{
        PreparedStatement ps;
        Connection con = connection;
        
        String updatestmt = "update sa_game set game_state=? where game_name=?";
        if(newState.equals("D")){
            updatestmt = "update sa_game set game_state=?, game_end_time=now() where game_name=?;";
        }
        
        con.setAutoCommit(false);
        ps = con.prepareStatement(updatestmt);
        ps.setString(1, newState);
        ps.setString(2, gameName);
        
        ps.executeUpdate();
        ps.close();
        con.commit();
        
        con.close();
        
    }
    
    public static void updatePlayerScore(Connection connection, long scoreNew, long playerid) throws SQLException{
        Connection con = connection;
        PreparedStatement ps = null;
        con.setAutoCommit(false);
       
        String stmt = "update sa_score set score_value = ? where score_user_id = ?";
        ps = con.prepareStatement(stmt);
        ps.setLong(1, scoreNew);
        ps.setLong(2, playerid);

        ps.executeUpdate();
        ps.close();
        con.commit();
        
        con.close();
    }
    
    public static void insertScoreTable(Connection connection, long userid) throws SQLException{
        Connection con = connection;
        PreparedStatement ps = null;
        
        String insertstmt = "insert into sa_score (score_user_id, score_value) values (?, 0::bigint)";

        con.setAutoCommit(false);
        ps = con.prepareStatement(insertstmt);
        ps.setLong(1, userid);

        ps.execute();
        
        con.commit();
        
        ps.close();
        con.close();       
    }
    
    public static long userIdFromUsername(Connection connection, String username) throws SQLException{
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        long id = -1L;
        
        String stmt = "select user_id from sa_user where user_name = ?";
        

        con.setAutoCommit(false);
        ps = con.prepareStatement(stmt);
        ps.setString(1, username);
        rs = ps.executeQuery();

        if(rs.next()){
            id = rs.getLong(UsernameTable.COL_USER_ID);
        }

        rs.close();
        ps.close();
        con.close();
        
        return id;
    }
    
    public static long selectUserScore(Connection connection, long userid) throws SQLException{
        long score = 0L;
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        String stmt = "select score_value from sa_score where score_user_id = ?";
        
        con.setAutoCommit(false);
        ps = con.prepareStatement(stmt);
        ps.setLong(1, userid);
        
        rs = ps.executeQuery();
        
        if(rs.next()){
            score = rs.getLong("score_value");
        }
        
        rs.close();
        ps.close();
        con.close();
        
        return score;
    }
    
    //TODO: make type safe
    public static ArrayList selectTopTenScores(Connection connection) throws SQLException{
        ArrayList results = new ArrayList();
        int numToReturn = 10;
        int currCount = 0;
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        
        String stmt = "select u.user_name as username," +
                " s.score_value as score from sa_user u " +
                " join sa_score s on user_id = score_user_id " +
                " order by s.score_value desc;";
        
        con.setAutoCommit(false);
        ps = con.prepareStatement(stmt);
        rs = ps.executeQuery();
        while(rs.next() && currCount < numToReturn){
            currCount++;
            results.add(rs.getString("username"));
            results.add(rs.getLong("score"));
        }
        
        rs.close();
        ps.close();
        con.close();
        
        return results;
    }
    
}
