output "ecs_cluster_id" {
  description = "The ID of the ECS cluster"
  value       = aws_ecs_cluster.this.id
}

output "ecs_cluster_name" {
  description = "The name of the ECS cluster"
  value       = aws_ecs_cluster.this.name
}

output "ecs_execution_role_arn" {
  description = "ECS task arn"
  value       = aws_iam_role.ecs_task_execution.arn
}
