# Dining-Philosophers
This application is created to demonstrate a well-known problem called [Dining philosophers problem](https://en.wikipedia.org/wiki/Dining_philosophers_problem) with two different solutions. You can check out [this](https://youtu.be/YaSXKAuz_GU) video for a more detailed explanation.

## DiningPhilosophers.Problem
The first project is where a deadlock situation is created on purpose to show the essence of the problem.

## DiningPhilosophers.Sol1
In the second project, I introduce a solution by using randomness and forcing the threads to release the resources they hold after a while.

## DiningPhilosophers.Sol2
In the last part, I avoid deadlocks using a more complex approach. Instead of letting threads fight over resources, I implement a kind of manager mechanism that controls which thread uses shared resources at a particular time.
