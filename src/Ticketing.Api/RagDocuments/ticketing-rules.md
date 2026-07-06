# Ticketing Large Rules

A ticket cannot be commented when it is closed. This rule protects the history of the ticket and avoids adding information after the business process has ended.

When a ticket is closed, the API must reject any new comment with a clear validation error. The error should explain that the ticket is already closed and cannot receive new comments.

Refresh tokens must always be stored hashed in the database. A refresh token must never be stored in plain text because database access would expose active sessions.

When a refresh token is used, the system must verify its hash, check its expiration date, and ensure that it has not already been revoked.

JWT renewal can fail when the refresh token is expired, invalid, revoked, or already used. In that case, the API must return an unauthorized response.

API errors must be returned using ProblemDetails. The response should contain a status code, a title, a detail message, and a trace identifier.

The trace identifier is useful for support because it allows developers to find the corresponding log entry quickly.

A closed ticket cannot be reopened without administrator permission. This prevents normal users from changing a completed workflow.

Only the author of a ticket or an administrator can update the ticket title or description.

When listing tickets, the API must support pagination to avoid returning too much data in one response.

For read-only queries, Entity Framework Core should use AsNoTracking to reduce memory usage and improve performance.

The system should avoid N plus one queries by using projection or Include only when necessary.

Domain rules must stay inside the domain layer. Infrastructure details such as SQL Server, Qdrant, or Ollama must not leak into the domain model.

The RAG ingestion pipeline extracts text from documents, splits the text into chunks, generates embeddings, and stores vectors in Qdrant.

Each chunk must keep its original document name, chunk index, content, and identifier.

Chunk overlap is important because it prevents losing context when an important sentence is split between two chunks.

Semantic search compares the user question vector with stored document vectors and returns the most relevant chunks.

The LLM should receive only the most useful chunks as context, not the full document.

If no relevant document is found, the assistant should say that it does not have enough information instead of inventing an answer.

This document is intentionally long enough to test whether the chunker creates multiple chunks with overlap.
