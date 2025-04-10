# Use a Node.js image to build and serve the frontend
FROM node:20-alpine

# Set the working directory
WORKDIR /app

# Copy package.json and install dependencies
COPY package.json yarn.lock ./
RUN yarn install

# Copy the rest of the application code
COPY . .

# Build the application
RUN yarn build

# Expose port 3000
EXPOSE 3000

# Start the application using a lightweight HTTP server
CMD ["npx", "serve", "-s", "dist", "-l", "3000"]