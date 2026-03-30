provider "aws" {
  region = var.region
}

# 1. Network Load Balancer (NLB)
resource "aws_lb" "this" {
  name               = "${var.project}-${var.environment}-nlb"
  internal           = false
  load_balancer_type = "network"
  subnets            = var.public_subnets

  enable_deletion_protection       = false
  enable_cross_zone_load_balancing = true

  tags = {
    Name = "${var.project}-${var.environment}-nlb"
  }
}

# 2. Target Group (UDP for Fargate)
resource "aws_lb_target_group" "udp_7777" {
  name        = "${var.project}-${var.environment}-tg-udp"
  port        = var.game_port
  protocol    = "UDP"
  vpc_id      = var.vpc_id
  target_type = "ip" 
  deregistration_delay = 30

  health_check {
    protocol = "TCP" 
    port     = var.health_check_port
    healthy_threshold   = 3
    unhealthy_threshold = 3
    interval            = 30
  }
}

# 3. Listener (Listens on 7777 and forwards to Target Group)
resource "aws_lb_listener" "udp_7777" {
  load_balancer_arn = aws_lb.this.arn
  port              = var.game_port
  protocol          = "UDP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.udp_7777.arn
  }
}

# 4. Route 53 (Domain Record) 
data "aws_route53_zone" "game_domain" {
  count = var.domain_name != "" ? 1 : 0
  name  = var.domain_name
}

resource "aws_route53_record" "game_record" {
  count   = var.domain_name != "" && var.subdomain != "" ? 1 : 0
  zone_id = data.aws_route53_zone.game_domain[0].zone_id
  
  name    = "${var.subdomain}.${var.domain_name}"
  type    = "A"

  alias {
    name                   = aws_lb.this.dns_name
    zone_id                = aws_lb.this.zone_id
    evaluate_target_health = true
  }
}