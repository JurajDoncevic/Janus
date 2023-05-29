
## MMW-driven data mesh experiments
Everything regarding the MMW-driven (Janus-driven) data mesh experiments can be found under the directory: `/experimentation/janus_data_mesh/`. The directory contains:
* the template files required for Docker Compose operations
* the Docker files themselves
* the `/mediation_scripts` directory containing individual mediation scripts for mediator components
* the time measuring experiment files under `/time_measuring`
* the databases used in the experiments are found under `/databases`

The containerization is set up so it loads and adapts these files for the required component images.


The data mesh that is being experimentally driven by Janus is pictured here:
<div align="center">
<img src="https://github.com/JurajDoncevic/Janus/blob/master/wiki/figures/dm_experiment_data_mesh.png?raw=true" width=100% alt="Experiment databases">
<br>
<caption><i>Experimental data mesh topology</i></caption>
</div>
<br/><br/>

### Running the experiments
To start the experimental data mesh, clone this repository and initiate the Docker Compose as follows:
```
$> git clone https://github.com/JurajDoncevic/Janus.git --branch master
$> cd ./experimentation/janus_data_mesh
$> docker compose build --no-cache
$> docker compose up -d listening_egress_mask subscriptions_egress_mask
```

To start the components required only for the data mesh response time tests run the following instructions:
```
$> git clone https://github.com/JurajDoncevic/Janus.git --branch master
$> cd ./experimentation/janus_data_mesh
$> docker compose build --no-cache
$> docker compose up -d customers_egress_mask subscriptions_egress_mask
```
The time measuring Python (v3) script can be located at `/time_measuring/time_measuring.py`. Local environment libraries are referenced in `requirements.txt`. To run the script run the following instructions:
```
$> cd ./experimentation/janus_data_mesh/time_measuring
$> python -m venv env
$> ./env/Scripts/activate
$> pip install -r requirements.txt
$> python time_measuring.py
```

### Experimental databases
The databases used as data sources in the DIP are described by the following unified logical diagram:

<div align="center">
<img src="https://github.com/JurajDoncevic/Janus/blob/master/wiki/figures/dm_experiment_databases_used.png?raw=true" width=100% alt="Experiment databases">
<br>
<caption><i>Experiment databases</i></caption>
</div>
<br/><br/>
