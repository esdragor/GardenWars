using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICandyable
{
    /// <returns>The maxCandy of the entity</returns>
    public int GetMaxCandy();

    /// <returns>The currentCandy of the entity</returns>
    public int GetCurrentCandy();

    /// <returns>The percentage of currentCandy on maxCandy of the entity</returns>
    public float GetCurrentCandyPercent();

    /// /// <summary>
    /// Sends an RPC to the master to set the entity's maxCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void RequestSetMaxCandy(int value);

    /// <summary>
    /// Sends an RPC to all clients to set the entity's maxCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SyncSetMaxCandyRPC(int value);

    /// <summary>
    /// Sets the entity's maxCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SetMaxCandyRPC(int value);

    public event GlobalDelegates.IntDelegate OnSetMaxCandy;
    public event GlobalDelegates.IntDelegate OnSetMaxCandyFeedback;

    /// <summary>
    /// Sends an RPC to the master to increase the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void RequestIncreaseMaxCandy(int amount);

    /// <summary>
    /// Sends an RPC to all clients to increase the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncIncreaseMaxCandyRPC(int amount);

    /// <summary>
    /// Increases the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void IncreaseMaxCandyRPC(int amount);

    public event GlobalDelegates.IntDelegate OnIncreaseMaxCandy;
    public event GlobalDelegates.IntDelegate OnIncreaseMaxCandyFeedback;

    /// <summary>
    /// Sends an RPC to the master to decrease the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void RequestDecreaseMaxCandy(int amount);

    /// <summary>
    /// Sends an RPC to all clients to decrease the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncDecreaseMaxCandyRPC(int amount);

    /// <summary>
    /// Decreases the entity's maxCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void DecreaseMaxCandyRPC(int amount);

    public event GlobalDelegates.IntDelegate OnDecreaseMaxCandy;
    public event GlobalDelegates.IntDelegate OnDecreaseMaxCandyFeedback;

    /// <summary>
    /// Sends an RPC to the master to set the entity's currentCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void RequestSetCurrentCandy(int value);

    /// <summary>
    /// Sends an RPC to all clients to set the entity's currentCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SyncSetCurrentCandyRPC(int value);

    /// <summary>
    /// Set the entity's currentCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SetCurrentCandyRPC(int value);

    public event GlobalDelegates.IntDelegate OnSetCurrentCandy;
    public event GlobalDelegates.IntDelegate OnSetCurrentCandyFeedback;

    /// <summary>
    /// Sends an RPC to the master to set the entity's currentCandy to a percentage of  its maxCandy.
    /// </summary>
    /// <param name="value">the value to set to</param>
    public void RequestSetCurrentCandyPercent(int value);

    /// <summary>
    /// Sends an RPC to all clients to set the entity's currentCandy to a percentage of  its maxCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SyncSetCurrentCandyPercentRPC(int value);

    /// <summary>
    /// Sets the entity's currentCandy to a percentage of its maxCandy.
    /// </summary>
    /// <param name="value">the value to set it to</param>
    public void SetCurrentCandyPercentRPC(int value);

    public event GlobalDelegates.IntDelegate OnSetCurrentCandyPercent;
    public event GlobalDelegates.IntDelegate OnSetCurrentCandyPercentFeedback;

    /// <summary>
    /// Sends an RPC to the master to increase the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void RequestIncreaseCurrentCandy(int amount);

    /// <summary>
    /// Sends an RPC to all clients to increase the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the increase amount</param>
    public void SyncIncreaseCurrentCandyRPC(int amount,int upgrade);

    /// <summary>
    /// Increases the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void IncreaseCurrentCandyRPC(int amount);

    public event GlobalDelegates.IntDelegate OnIncreaseCurrentCandy;
    public event GlobalDelegates.IntDelegate OnIncreaseCurrentCandyFeedback;

    /// <summary>
    /// Sends an RPC to the master to decrease the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void RequestDecreaseCurrentCandy(int amount);

    /// <summary>
    /// Sends an RPC to all clients to decrease the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void SyncDecreaseCurrentCandyRPC(int amount);

    /// <summary>
    /// Decreases the entity's currentCandy.
    /// </summary>
    /// <param name="amount">the decrease amount</param>
    public void DecreaseCurrentCandyRPC(int amount);

    public event GlobalDelegates.IntDelegate OnDecreaseCurrentCandy;
    public event GlobalDelegates.IntDelegate OnDecreaseCurrentCandyFeedback;
}