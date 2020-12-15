using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BulletFactory 
{
    // step 1. private static instance
    private static BulletFactory m_instance = null;

    // prefab references
    private GameObject regularBullet;
    private GameObject spikedBullet;

    // game controller reference
    private GameController gameController;

    // step 2. make constructor private
    private BulletFactory()
    {
        _Initialize();
    }

    // step 3. make a pubic static creational method for class access
    public static BulletFactory Instance()
    {
        if (m_instance == null)
        {
            m_instance = new BulletFactory();
        }

        return m_instance;
    }

    /// <summary>
    /// This method initializes bullet prefabs
    /// </summary>
    private void _Initialize()
    {
        // 4. create a Resources folder
        // 5. move our Prefabs folder into the Resources folder
        regularBullet = Resources.Load("Prefabs/Bullet") as GameObject;
        spikedBullet = Resources.Load("Prefabs/SpikedBall") as GameObject;

        gameController = GameObject.FindObjectOfType<GameController>();
    }

    /// <summary>
    /// This method creates a bullet of the specified enumeration
    /// </summary>
    /// <param name="type"></param>
    /// <returns> GameObject </returns>
    public GameObject createBullet(BulletType type = BulletType.REGULAR)
    {

        GameObject tempBullet = null;
        switch (type)
        {
            case BulletType.REGULAR:
                tempBullet = MonoBehaviour.Instantiate(regularBullet);
                tempBullet.GetComponent<BulletController>().damage = 10;
                break;

            case BulletType.SPIKED:
                tempBullet = MonoBehaviour.Instantiate(spikedBullet);
                tempBullet.GetComponent<GrenadeBehaviour>().damage = 10;
                break;
        }

        if (gameController == null)
        {
            gameController = GameObject.FindObjectOfType<GameController>();
        }

        tempBullet.transform.parent = gameController.gameObject.transform;
        tempBullet.SetActive(false);

        return tempBullet;
    }
}
