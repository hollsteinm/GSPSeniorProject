/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;
import com.gspteama.extensions.MultiHandler;
import com.gspteama.gamedriver.*;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author Martin
 */
public class DBService {
    public static void UpdatePlayerScore(Connection connection, Player player){
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        long id;
        float score;
        
        String stmtselect = "select u.user_id, s.score_value from " +
                "sa_user u join score s on s.score_user_id = u.user_id " +
                "where u.user_name = ?";
                
        String stmtupdate = "update sa_score set score_value = ? where " +
                "score_user_id = ?";
        
        try{
            con.setAutoCommit(false);

            ps = con.prepareStatement(stmtselect);
            ps.setString(1, player.getUsername());

            rs = ps.executeQuery();

            if(rs.next()){
                id = rs.getLong(UsernameTable.COL_USER_ID);
                score = rs.getFloat(ScoreTable.COL_SCORE_VALUE);

                score += player.getScore();
                player.setScore(0.0f);

                ps = con.prepareStatement(stmtupdate);
                ps.setFloat(1, score);
                ps.setLong(2, id);

                ps.executeUpdate();
            }
            
            rs.close();
            ps.close();
        } catch (Exception e){
            Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
        } finally{
            if(con != null){
                try{
                    con.close();
                }catch(Exception e){
                    Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
                }
            }
        }
    }
    
    public static void insertScoreTable(Connection connection, long userid){
        Connection con = connection;
        PreparedStatement ps = null;
        
        String insertstmt = "insert into sa_score (score_user_id) values (?)";
        
        try{
            con.setAutoCommit(false);
            ps = con.prepareStatement(insertstmt);
            ps.setLong(1, (long)userid);
            
            ps.executeUpdate();
            ps.close();
        }catch(Exception e){
            Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
        } finally {
            if(con!=null){
                try{
                    con.close();
                }catch(Exception e){
                    Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
                }
            }
        }        
    }
    
    public static long userIdFromUsername(Connection connection, String username){
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        long id = -1L;
        
        String stmt = "select user_id from sa_user where user_name = ?";
        
        try{
            con.setAutoCommit(false);
            ps = con.prepareStatement(stmt);
            ps.setString(1, username);
            rs = ps.executeQuery();
            
            if(rs.next()){
                id = rs.getLong(UsernameTable.COL_USER_ID);
            }
            
            rs.close();
            ps.close();
        } catch (Exception e){
            Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
        } finally {
            if(con != null){
                try{
                    con.close();
                }catch(Exception e){
                    Logger.getLogger(DBService.class.getName()).log(Level.SEVERE, null, e);
                }
            }
        }
        
        return id;
    }
}