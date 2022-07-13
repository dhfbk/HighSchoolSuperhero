using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class HTMLReceiver : MonoBehaviour
{
    public Player agent;
    public Login login;
    // Start is called before the first frame update
    void StoreEmail(string email)
    {
        print(email);
        agent.id = Player.Md5(email);
        login.done = true;
    }
}
