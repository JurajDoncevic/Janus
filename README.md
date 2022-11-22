# Janus

## What is Janus?
Janus is a **mask-mediator-wrapper** architecture for (not just) heterogeneous data source integration system.

Janus is a prototype of a heterogeneous data source integration system. It is being developed as part
of my PhD thesis titled *"Extension of the mediator-wrapper architecture for heterogeneous data source
integration by adding a mask component"*.

The Janus system is built following the mask-mediator-wrapper architecture. This architecture is an extension of the well-known mediator-wrapper architecture. The addition of a mask component type allows uniform and simplified development of data and metadata representations, as well as improving system maintainability. The idea is to implement the mask component using bidirectionalization for data transformation. This will decrease the effort required for developing a mask kind.

Since there is a lot of talk about masks and bidirectionalization, the system is named **Janus**;
referencing the old roman god of duality, gates and transitions - *Janus Bifrons*.

![Janus Bifrons](/wiki/figures/god_janus.jpg)  
*Statue of Janus Bifrons - the two-faced roman god (source: Wikipedia)*

## Janus' design
The mask-mediator-wrapper (MMW) architecture on which Janus is based is an extension of the mediator-wrapper (MW) architectural pattern.

![MW pattern](/wiki/figures/mediator_wrapper_pattern.png)  
*The mediator-wrapper pattern*

The MMW architecture extends the mediator-wrapper architecture by the addition of a mask component type. Masks are used for schema, query and data representation. This concern was usually assigned to specialized client applications built for those specific integration systems. Masks provide the ability to support a variety of user applications that are not inherently designed for the integration system itself. An example of a MMW system is shown here:

![MMW example](/wiki/figures/mask_mediator_wrapper_layered_arch.png)

Janus' development extensively uses my **[Functional extensions base library](https://github.com/JurajDoncevic/FunctionalExtensions)**. Feel free to take a look at the latest nuget package :-)

## Research papers regarding MMW and its capabilities
**[Mask-Mediator-Wrapper: A revised mediator-wrapper architecture for heterogeneous data source integration](https://doi.org/10.48550/arXiv.2208.12319)**
**[Mask-Mediator-Wrapper architecture as a Data Mesh driver](https://doi.org/10.48550/arXiv.2209.04661)**