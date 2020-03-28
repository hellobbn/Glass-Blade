﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCharacter : MonoBehaviour
{
  public Rigidbody weapon;  //武器
  public Transform muzzle;  //辅助武器旋转变量
  Rigidbody weaponInstance; //武器的实例变量

  int tempRoration;
  public float attackTime;  //攻击持续时间

  bool isAlive = true;
  bool attacking = false;

  //黑白渲染组件
  BlackAndWhite baw;
  //死亡倒计时组件
  Text counter;
  //死亡倒地段数
  public int DeathRotateTimes = 10;
  //复活时间
  public int DeathTime = 10;
  //全局变量
  int rotatedtimes;
  int counttime;

  public bool isProtected = false; //无敌状态标识
  public bool isJustAlive = false; //是否刚刚复活标识
  int protecttime;
  public int protecttimeMAX = 1;


  CharacterController cc;
  public void Attack() {
    if (!isAlive) return;
    if (attacking) return;
    attacking = true;
    // 武器出现位置
    muzzle.localPosition = this.transform.localPosition + new Vector3(0,1,0)+this.transform.forward;
    weaponInstance = Instantiate(weapon, muzzle.localPosition, this.transform.rotation) as Rigidbody;
    // 武器旋转表示攻击动作
    weaponInstance.angularVelocity = this.transform.right * 1 * 2;
    tempRoration = 0;
    Invoke("RefreshAttack", attackTime);
  }
  
  void RefreshAttack() {
    attacking = false;
  }

  public void TakeDamage() {
    Debug.Log("damage");
    // 非刚复活无敌状态且活着，才能受到伤害死亡
    if (!isProtected && !isJustAlive && isAlive) {
      Death();
    }
  }

  public void Death() {
    //**灰屏
    baw.setDeath();
    isAlive = false;
    //**倒下,先停1s,用2s分段倒下
    rotatedtimes = 0;
    InvokeRepeating("DeathRotate", 1f, (float)(2.0 / DeathRotateTimes));

    //**倒计时
    counttime = DeathTime;
    InvokeRepeating("CountDown", 0f, 1.0f);
  }

  //**大风车吱呀吱悠悠的转~
  void DeathRotate() {
    //Debug.Log("time = "+Time.time);
    transform.Rotate(90 / DeathRotateTimes, 0, 0);
    rotatedtimes++;
    if (rotatedtimes >= DeathRotateTimes) {
      CancelInvoke("DeathRotate");
    }
  }

  //**倒计时
  void CountDown() {
    if (counttime == 0)  //复活
    {
      counter.text = "";
      CancelInvoke("CountDown");
      Invoke("Relive", 1f);
    }
    else {
      counter.text = "剩余时间:" + counttime;
      counttime = counttime - 1;
    }
  }

  public void Relive() {
    //Debug.Log("relive time = " + Time.time);
    baw.setLive();
    ///TODO,next one-lhl
    isAlive = true;
    var reliver = Random.Range(-180.0f, 180.0f);
    transform.rotation = Quaternion.Euler(0, reliver, 0);
    var relivex = Random.Range(-40.0f, 40.0f);
    var relivez = Random.Range(-40.0f, 40.0f);
    transform.position = new Vector3(relivex, 1, relivez);
    //relive-protect
    protecttime = protecttimeMAX;
    if (protecttime <= 0) {
      isJustAlive = false;
    }
    else {
      isJustAlive = true;
      InvokeRepeating("protectDecrease", 0f, 1.0f);
    }
  }
  void protectDecrease() {
    if (protecttime == 0) {
      isJustAlive = false;
      CancelInvoke("protectDecrease");
    }
    else {
      protecttime--;
    }
  }

  // Start is called before the first frame update
  void Start() {
    cc = GetComponent<CharacterController>();
    baw = GameObject.FindObjectOfType<BlackAndWhite>();
    counter = GameObject.FindObjectOfType<Text>();
    counter.text = "";
    if (!baw || !counter) {
      Debug.Log("can't find, T_T");
    }
  }
    
  // Update is called once per frame
  void Update() {

    
  }
}
