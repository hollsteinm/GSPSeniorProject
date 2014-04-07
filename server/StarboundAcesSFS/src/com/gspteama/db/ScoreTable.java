/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package com.gspteama.db;

/**
 *
 * @author Martin
 */
public class ScoreTable {
    public static final String      TABLE_NAME          = "sa_score";
    public static final String      COL_SCORE_ID        = "score_id";
    public static final String      COL_SCORE_VALUE     = "score_value";
    public static final String      COL_SCORE_USER_ID   = "score_user_id";
    
    private long    score   = 0;
    private int     userid  ;
    private int     id      ;
    
    public ScoreTable(long score, int userid, int id){
        this.score = score;
        this.userid = userid;
        this.id = id;
    }
    
    //Increments and returns new score
    public long incrementScore(long value){
        score += value;
        return score;
    }
    
    public long getScore() {
        return score;
    }

    public void setScore(long score) {
        this.score = score;
    }

    public int getUserid() {
        return userid;
    }

    public void setUserid(int userid) {
        this.userid = userid;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }
    
}