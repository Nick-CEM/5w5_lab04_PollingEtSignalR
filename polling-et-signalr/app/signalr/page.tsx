"use client";

import React from "react";
import { useEffect } from "react";
import { UselessTask } from "../models/UselessTask";
import TaskView from "../_components/tasks-view";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";

export default function Home() {

  const [tasks, setTasks] = React.useState<UselessTask[]>([]);
  const [hubConnection, setHubConnection] = React.useState<HubConnection>();

  useEffect(() => {
      connecttohub();
    }, []);

  function connecttohub() {
    // TODO On doit commencer par créer la connexion vers le Hub
    let newHubConnection = new HubConnectionBuilder()
                              .withUrl("https://localhost:7289/MyHub")
                              .build();

    // TODO On peut commencer à écouter pour les évènements qui vont déclencher des callbacks
    newHubConnection.on("TaskList", (data) => {
      console.log(data)
      setTasks(data)
    })

    newHubConnection.on("UserCount", (data) => {
      console.log(data)
    })

    // TODO On doit ensuite se connecter
    newHubConnection
        .start()
        .then(() => {
          console.log("La connection est active !")
        })
        .catch(err => console.log('Error while starting connection: ' + err));
    
    setHubConnection(newHubConnection)
  }

  function onTaskToggle(id: number) {
    // TODO On invoke la méthode pour compléter une tâche sur le serveur
    // CompletedTask est la méthode du back-end à laquelle on fait référence
    // donc écrire PAREIL
    hubConnection!.invoke("CompletedTask", id)
  }

  function handleTaskAdd(taskName: string) {
    // TODO On invoke la méthode pour ajouter une tâche sur le serveur
    // AddTask est la méthode du back-end à laquelle on fait référence
    // donc écrire PAREIL
    hubConnection!.invoke("AddTask", taskName)
  }

  return (
    <div className="p-4">
        <h1>SignalR!</h1>
        <TaskView 
          tasks={tasks} 
          onTaskAdd={handleTaskAdd}
          onTaskToggle={onTaskToggle}
        />
    </div>
  );
}