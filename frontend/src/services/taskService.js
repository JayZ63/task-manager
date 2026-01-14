import axios from 'axios';
const API_URL = `${process.env.NEXT_PUBLIC_API_URL}/api/task`;


export const getAllTasks = async () => axios.get(API_URL);
export const addTask = async (task) => axios.post(API_URL, task);
export const updateTask = async (taskId, updatedTask) => axios.put(`${API_URL}/${taskId}`, updatedTask);
export const deleteTask = async (taskId) => axios.delete(`${API_URL}/${taskId}`);