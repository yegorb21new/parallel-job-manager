# Parallel Job Manager

A simplified job orchestration system that simulates executing workloads across multiple compute queues.

## Overview

This application models a backend system responsible for:

- receiving job definitions
- assigning jobs to available compute queues
- executing workloads
- tracking job status and results
- producing a final run summary

Each job:
- is derived from a business input definition
- is assigned to a queue
- runs for a simulated duration
- returns a success/failure result
- records execution metadata (status, exit code, duration)

---

## Current Features

- Job creation from input definitions (currently hardcoded)
- Queue-based job assignment
- Sequential job execution
- Job lifecycle tracking:
  - Pending → Running → Success / Failure
- Simulated execution timing and results
- Timestamped console logging
- End-of-run summary:
  - total jobs
  - success/failure counts
  - failed job IDs

---

## Example Output

```
[4/15/2026 1:19:40 PM]: Job number 1 Status=Running on queue ESFS_001
[4/15/2026 1:19:41 PM]: Job number 1 on queue ESFS_001 ran for 864 ms and finished with result=23. Status=Success
...
[4/15/2026 1:19:45 PM]: ======================== RUN SUMMARY ========================
[4/15/2026 1:19:45 PM]: Total Jobs: 5
[4/15/2026 1:19:45 PM]: Succeeded: 4
[4/15/2026 1:19:45 PM]: Failed: 1
[4/15/2026 1:19:45 PM]: Failed Job IDs: 3
```

---

## Architecture (Current)

- `Program.cs` → orchestration loop
- `Job.cs` → job state and metadata
- `GridQueue.cs` → queue resource and execution logic
- `InputDef.cs` → job input definition
- `Helpers.cs` → shared enums (e.g., JobStatus)

---

## Roadmap

Planned enhancements:

### Execution & Scheduling
- Parallel job execution (task-based)
- Queue capacity management (multi-slot / core-based scheduling)
- Job retry logic and failure handling
- Dependency handling between jobs

### Input & Integration
- Config-driven input (JSON or file-based)
- API layer for job submission and control

### Monitoring & UI
- Web-based dashboard for:
  - job monitoring
  - queue utilization
  - run summaries
- Potential support for job submission via UI

### System Design
- Separation of concerns into reusable components (e.g., logging, execution engine)
- Exploration of service-based architecture for scalability and modularity

---

## Purpose

This project demonstrates core backend and system design concepts, including:

- job orchestration
- resource scheduling
- state management
- incremental system evolution

---

## Notes

This is an early-stage implementation focused on correctness and clarity before introducing concurrency and more advanced scheduling behavior.