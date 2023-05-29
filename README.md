# Janus

## What is Janus?
Janus is a **mask-mediator-wrapper** architecture for (not just) heterogeneous data source integration system.

<div align="center">
<img src="https://github.com/JurajDoncevic/Janus/blob/master/wiki/figures/janus_logo.png?raw=true" width=40% alt="Janus project logo">
<br>
<caption><i>Janus project logo</i></caption>
</div>
<br/><br/>

Janus is a **prototype** of a heterogeneous data source integration system. It can also be used as a versatile data management system, capable of emulating other architectures. It is being developed as part
of my PhD thesis titled *"Extension of the mediator-wrapper architecture for heterogeneous data source
integration by adding a mask component"*.

The Janus system is built following the mask-mediator-wrapper architecture. This architecture is an extension of the well-known mediator-wrapper architecture. The addition of a mask component type allows uniform and simplified development of data and metadata representations, as well as improving system maintainability. The idea is to use bidirectionalization for data translation when implementing the mask component. This will decrease the effort required for developing a mask kind.

Since there is a lot of talk about masks and a plan to use bidirectionalization, the system is named **Janus**;
referencing the old roman god of duality, gates and transitions - *Janus Bifrons*.

## Janus' design
The mask-mediator-wrapper (MMW) architecture on which Janus is based is an extension of the mediator-wrapper (MW) architectural pattern.

<div align="center">
<img src="https://github.com/JurajDoncevic/Janus/blob/master/wiki/figures/mediator_wrapper_pattern.png?raw=true" width=40% alt="MW pattern">
<br>
<caption><i>The mediator-wrapper pattern</i></caption>
</div>
<br/><br/>

The MMW architecture extends the mediator-wrapper architecture by the addition of a mask component type. Masks are used for schema, query and data representation. This concern was usually assigned to specialized client applications built for specific integration systems. Masks provide the ability to support a variety of user applications that are not inherently designed for the integration system itself. An example of a MMW system is shown here:

<div align="center">
<img src="https://github.com/JurajDoncevic/Janus/blob/master/wiki/figures/mmw_architecture_example.png?raw=true" width=40% alt="MMW wxample">
<br>
<caption><i>MMW example</i></caption>
</div>
<br/><br/>


## MMW-driven data mesh experiments
The Janus system has shown that an MMW system can drive a data mesh. The experiment files are located [HERE](https://github.com/JurajDoncevic/Janus/blob/master/experimentation/janus_data_mesh/).
The description of those experiments, and the way to reproduce them is described <a href="https://github.com/JurajDoncevic/Janus/blob/master/wiki/data_mesh_experiments.md">HERE</a>.


## Research papers regarding MMW and its capabilities
**[Mask-Mediator-Wrapper: A revised mediator-wrapper architecture for heterogeneous data source integration](https://doi.org/10.3390/app13042471)**
**[Mask-Mediator-Wrapper architecture as a Data Mesh driver](https://doi.org/10.48550/arXiv.2209.04661)**


