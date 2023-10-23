#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Use an existing image as a base
FROM postgres:latest

# Set environment variables (optional)
ENV POSTGRES_DB=TheTvDb
ENV POSTGRES_USER=Memnoch
ENV POSTGRES_PASSWORD=WTF

# Copy initialization scripts or SQL files (optional)
COPY ./init.sql /docker-entrypoint-initdb.d/
