/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;
import com.gspteama.gamedriver.*;
import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.ArrayList;

/**
 *
 * @author Martin
 */
public class DBService {
    
    public static void insertNewGame(Connection connection, int instanceid){
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        
        String stmtinsert = "instert into sa_games (";
    }
    
    
    public static void updatePlayerScore(Connection connection, Player player) throws SQLException{
        Connection con = connection;
        PreparedStatement ps = null;
        ResultSet rs = null;
        long id;
        long score;
        
        String stmtselect = "select u.user_id, s.score_value from " +
                "sa_user u join score s on s.score_user_id = u.user_id " +
                "where u.user_name = ?";
                
        String stmtupdate = "update sa_score set score_value = ? where " +
                "score_user_id = ?";
        
        
        con.setAutoCommit(false);

        ps = con.prepareStatement(stmtselect);
        ps.setString(1, player.getUsername());

        rs = ps.executeQuery();
        rs.close();

        if(rs.next()){
            id = rs.getLong(UsernameTable.COL_USER_ID);
            score = rs.getLong(ScoreTable.COL_SCORE_VALUE);

            score += player.getScore();
            player.setScore(0L);

            ps = con.prepareStatement(stmtupdate);
            ps.setLong(1, score);
            ps.setLong(2, id);

            ps.executeUpdate();
            con.commit();
        }

        ps.close();
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