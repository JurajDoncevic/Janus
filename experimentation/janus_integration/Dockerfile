### GENERIC BUILD STAGE
FROM mcr.microsoft.com/dotnet/sdk:6.0 as janus-generic-build
RUN git clone https://github.com/JurajDoncevic/Janus.git /Janus 
WORKDIR /Janus
RUN git checkout development
RUN mkdir /App && mkdir /Scripts

ARG PROJECT_NAME
ARG COMPONENT_TYPE

WORKDIR /Janus
RUN dotnet restore Janus/${PROJECT_NAME}/${PROJECT_NAME}.csproj
RUN echo "" > Janus/${PROJECT_NAME}/appsettings.Development.json
RUN dotnet publish Janus/${PROJECT_NAME} -c Release -o /App -p:EnablePreviewFeatures=true
RUN dotnet dev-certs https


### WRAPPER DEPLOYMENT STAGE
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as janus-sqlite-wrapper-deploy

ARG PROJECT_NAME="Janus.Wrapper.Sqlite.WebApp"
ARG COMPONENT_TYPE="SqliteWrapper"
ARG DB_NAME
ARG DB_DIR="/Database/"
ENV PROJECT_NAME=${PROJECT_NAME}
ENV COMPONENT_TYPE=${COMPONENT_TYPE}
ENV DB_NAME=${DB_NAME}
ENV DB_DIR=${DB_DIR}

COPY --from=janus-generic-build /App /App
COPY --from=janus-generic-build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

COPY reformat_remote_points.sh /Scripts/reformat_remote_points.sh
COPY wrapper_sqlite_startup.sh /Scripts/wrapper_sqlite_startup.sh
RUN chmod +x /Scripts/wrapper_sqlite_startup.sh

COPY ./databases/${DB_NAME} ${DB_DIR}${DB_NAME}
COPY appsettings.${COMPONENT_TYPE}.json.template /App/appsettings.json.template
RUN apt-get update && apt-get install -y gettext

RUN chmod +x /App/${PROJECT_NAME}
WORKDIR /App
ENTRYPOINT bash /Scripts/wrapper_sqlite_startup.sh ${PROJECT_NAME}


### MEDIATOR DEPLOYMENT STAGE
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as janus-mediator-deploy
ARG PROJECT_NAME="Janus.Mediator.WebApp"
ARG COMPONENT_TYPE="Mediator"
ENV PROJECT_NAME=${PROJECT_NAME}
ENV COMPONENT_TYPE=${COMPONENT_TYPE}

COPY --from=janus-generic-build /App /App
COPY --from=janus-generic-build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

COPY reformat_remote_points.sh /Scripts/reformat_remote_points.sh
COPY reformat_node_ids.sh /Scripts/reformat_node_ids.sh
COPY mediator_startup.sh /Scripts/mediator_startup.sh
RUN chmod +x /Scripts/mediator_startup.sh

COPY appsettings.${COMPONENT_TYPE}.json.template /App/appsettings.json.template
RUN apt-get update && apt-get install -y gettext

RUN chmod +x /App/${PROJECT_NAME}
WORKDIR /App
ENTRYPOINT bash /Scripts/mediator_startup.sh ${PROJECT_NAME}


### MASK DEPLOYMENT STAGE
FROM mcr.microsoft.com/dotnet/aspnet:6.0 as janus-mask-deploy

ARG PROJECT_NAME="Janus.Mask.WebApi.WebApp"
ARG COMPONENT_TYPE="WebApiMask"
ENV PROJECT_NAME=${PROJECT_NAME}
ENV COMPONENT_TYPE=${COMPONENT_TYPE}

COPY --from=janus-generic-build /App /App
COPY --from=janus-generic-build /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

COPY reformat_remote_points.sh /Scripts/reformat_remote_points.sh
COPY mask_webapi_startup.sh /Scripts/mask_webapi_startup.sh
RUN chmod +x --recursive /Scripts

COPY appsettings.${COMPONENT_TYPE}.json.template /App/appsettings.json.template
RUN apt-get update && apt-get install -y gettext

RUN chmod +x /App/${PROJECT_NAME}
WORKDIR /App
ENTRYPOINT bash /Scripts/mask_webapi_startup.sh ${PROJECT_NAME}
