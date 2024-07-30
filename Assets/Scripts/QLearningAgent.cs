using System;
using System.Collections.Generic;
using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class QLearningAgent : Agent
{
    private GameManager gameManager;
    private Dictionary<string, Dictionary<GameObject, float>> QTable = new();
    private float alpha = 0.1f;
    private float gamma = 0.9f;
    private float epsilon = 1.0f;
    private float minEpsilon = 0.1f;
    private float epsilonDecay = 0.995f;
    private GameObject currentPlayer;

    public override void OnEpisodeBegin()
    {
        SceneManager.LoadScene("SampleScene");
        currentPlayer = GameManager.Instance.CurrentPlayer;
        epsilon = Mathf.Max(minEpsilon, epsilon * epsilonDecay);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(currentPlayer.transform.position);

        var raySensor = GetComponent<RayPerceptionSensorComponentBase>();
        var rayObservations = raySensor.GetRayPerceptionInput();
        foreach (var observation in rayObservations)
        {
            sensor.AddObservation(observation);
        }
        var enemies = GameManager.Instance.Players.FindAll(
            go => go.GetComponent<Player>().team != currentPlayer.GetComponent<Player>().team);
        foreach (var enemy in enemies)
        {
            sensor.AddObservation(enemy.transform.position);
        }
    }

    private string GetStateKey(GameObject currPlayer, List<GameObject> enemies)
    {
        var playerPos = $"{currPlayer.transform.position.x},{currPlayer.transform.position.y}";
        var enemyPositions = enemies.Select(enemy => 
            $"{enemy.transform.position.x},{enemy.transform.position.y}").ToList();

        return playerPos + "," + string.Join(",", enemyPositions);
    }

    private GameObject SelectTarget()
    {
        var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
        var enemies = GameManager.Instance.Players.FindAll(
            go => go.GetComponent<Player>().team != currentTeam);

        var stateKey = GetStateKey(currentPlayer, enemies);

        if (Random.value < epsilon)
        {
            return enemies[Random.Range(0, enemies.Count)];
        }
        else
        {
            return GetBestTarget(stateKey);
        }
    }

    private GameObject GetBestTarget(string stateKey)
    {
        if (QTable.ContainsKey(stateKey))
        {
            return QTable[stateKey].OrderByDescending(kvp => kvp.Value).First().Key;
        }

        return null;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var selectedTarget = SelectTarget();

        if (!selectedTarget) // then choose target randomly
        {
            var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
            var enemies = GameManager.Instance.Players.FindAll(
                go => go.GetComponent<Player>().team != currentTeam);
            
            if (enemies.Count > 0)
            {
                selectedTarget = enemies[Random.Range(0, enemies.Count)];
            }
            else
            {
                AddReward(-0.05f); // to sie nigdy NIE POWINNO WYDARZYĆ ALE JAKBY TAK TO UJEBIE MU NAGRODĘ
                return;
            }
        }
        
        var reward = TakeAction(selectedTarget);
        AddReward(reward);
        UpdateQTable(selectedTarget, reward);
    }


    private void UpdateQTable(GameObject action, float reward)
    {
        var currentTeam = GameManager.Instance.CurrentPlayer.GetComponent<Player>().team;
        var enemies = GameManager.Instance.Players.FindAll(
            go => go.GetComponent<Player>().team != currentTeam);
        
        var stateKey = GetStateKey(currentPlayer, enemies);
        if (!QTable.ContainsKey(stateKey))
        {
            QTable[stateKey] = new Dictionary<GameObject, float>();
            foreach (var enemy in enemies)
            {
                QTable[stateKey][enemy] = 0;
            }
        }

        QTable[stateKey][action] += alpha * (reward + gamma * GetMaxQValue(stateKey) - QTable[stateKey][action]);
    }

    private float GetMaxQValue(string stateKey)
    {
        var maxQValue = float.MinValue;

        foreach (var kvp in QTable[stateKey])
        {
            if (kvp.Value > maxQValue)
            {
                maxQValue = kvp.Value;
            }
        }

        return maxQValue;
    }

    private float TakeAction(GameObject selectedTarget)
    {
        float enemyHealth = selectedTarget.GetComponent<Player>().Health;
        GameManager.Instance.ShootTarget(selectedTarget);
        float enemyHealthAfterDamage = selectedTarget.GetComponent<Player>().Health;
        var healthLost = enemyHealth - enemyHealthAfterDamage;
        var reward = Math.Max(0, Math.Min(1, healthLost / enemyHealth));
        if (healthLost <= 0)
        {
            return -1;
        }

        return reward;
    }
}
