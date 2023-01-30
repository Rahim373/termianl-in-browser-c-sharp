FROM node:16-alpine

WORKDIR /app

COPY . .

RUN npm ci

RUN npm run build

ENV NODE_ENV production
ENV REACT_APP_HUB_URL $REACT_APP_HUB_URL

EXPOSE 3000

CMD [ "npx", "serve", "build" ]