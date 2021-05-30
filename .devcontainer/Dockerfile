FROM mcr.microsoft.com/dotnet/sdk:5.0-alpine

RUN apk add --no-cache bash nodejs nodejs-npm

RUN sed -i -e "s/bin\/ash/bin\/bash/" /etc/passwd

RUN apk --update add tzdata && \
    cp /usr/share/zoneinfo/Asia/Tokyo /etc/localtime && \
    apk del tzdata
