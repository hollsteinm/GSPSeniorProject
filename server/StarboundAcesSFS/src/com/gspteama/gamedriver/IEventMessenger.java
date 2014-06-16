/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package com.gspteama.gamedriver;

/**
 *
 * @author Martin
 */
public interface IEventMessenger {
    public void Register(IEventListener listener);
    public void Unregister(IEventListener listener);
    public void OnEvent(String event, Object data);
    
}
