using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

public class IAEnemigo : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] private Transform player;
    [SerializeField] private CamaraNino efectoCamara;

    [SerializeField] private List<Transform> puntosPatrulla;
    [SerializeField] private float tiempoEsperaPatrulla = 2f;
    private int indiceActual = 0;
    private bool esperando = false;
    private bool atacando = false;

    [SerializeField] private float velocidadNormal = 2.5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }

    private void Start()
    {
        if (puntosPatrulla.Count > 0) ActualizarDestinoPatrulla();
    }

    private void Update()
    {
        if (atacando) return;
        {
            if (!esperando) Patrullar();
        }

        AplicarGiroManual();
    }

    private void AplicarGiroManual()
    {
        Vector3 direccionGiro = agent.velocity;
        if (atacando) direccionGiro = (player.position - transform.position);

        if (direccionGiro.sqrMagnitude > 0.1f)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(new Vector3(direccionGiro.x, 0, direccionGiro.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, Time.deltaTime * 10f);
        }
    }

    void Patrullar()
    {
        agent.isStopped = false;
        agent.speed = velocidadNormal;
        if (!agent.pathPending && agent.remainingDistance < 0.5f) StartCoroutine(PausaEnPunto());
    }

    IEnumerator PausaEnPunto()
    {
        esperando = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(tiempoEsperaPatrulla);
        indiceActual = (indiceActual + 1) % puntosPatrulla.Count;
        ActualizarDestinoPatrulla();
        agent.isStopped = false;
        esperando = false;
    }

    void ActualizarDestinoPatrulla()
    {
        agent.SetDestination(puntosPatrulla[indiceActual].position);
    }
}