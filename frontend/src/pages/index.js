import { getAllTasks, addTask, updateTask, deleteTask } from "@/services/taskService";
import { useEffect, useState } from "react";


export default function Home() {

    const [tasks, setTasks] = useState([]);
    const [newTitle, setNewTitle] = useState("");

    useEffect(() => {
        // Fetch tasks from the backend when the component mounts
        fetchTasks();
    }, []);

    const fetchTasks = async () => {
        // Logic to fetch tasks from the backend
        const res = await getAllTasks();
        setTasks(res.data);
    }

    const handleAddTask = async () => {
        // Logic to add a new task
        if (!newTitle || newTitle.trim() === "") return;
        await addTask({ title: newTitle, isCompleted: false });
        setNewTitle("");
        fetchTasks();
    }

    const handleToggle = async (task) => {
        // Logic to toggle task completion
        debugger;
        await updateTask(task.id, { ...task, isCompleted: !task.isCompleted });
        fetchTasks();
    }

    const handleDelete = async (taskId) => {
        // Logic to delete a task
        debugger;
        await deleteTask(taskId);
        fetchTasks();
    }

  return (
    <div style={{ padding: "2rem" }}>
      <h1>Task Manager</h1>
      <input value={newTitle} onChange={(e) => setNewTitle(e.target.value)} placeholder="New task" />
      <button onClick={handleAddTask}>Add</button>
      <ul>
        {tasks.map(t => (
          <li key={t.id}>
            <span style={{ textDecoration: t.isCompleted ? "line-through" : "none" }}>{t.title}</span>
            <button onClick={() => handleToggle(t)}>Toggle</button>
            <button onClick={() => handleDelete(t.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}