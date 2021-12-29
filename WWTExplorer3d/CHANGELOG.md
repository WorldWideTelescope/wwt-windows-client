# WWTExplorer 6.0.900.0 (2021-12-29)

This is a trial release created using automated Azure Pipelines processing. Use
it at your own risk.

- Update the splash screen and refresh credits (#190, @pkgw)
- Homogenize "AAS WorldWide Telescope" branding more thoroughly, including app
  installation and data storage directories (#190, #177, #176)
- Use a new login API endpoint so that we can push updates to this series of
  clients without triggering updates in the older install base (#189,
  @Carifio24)
- Make it possible to adjust VOTable layer properties via the layer manager, and
  add support for colormaps (#187, @Carifio24)
- Respect the units when creating fixed spherical reference frames (#186,
  @Carifio24)
- Respect the units in trajectory reference frames (#185, @Carifio24)
- Fix auto-hiding tabs when the Layer Manager is closed (#182, @Carifio24)
- Support distance axes in VOTable layers (#181, @Carifio24)
- Don't continually try to download errored-out HEALPix tiles (#171, @imbasimba)
- Only hide eclipsed moons in realistic-lighting mode (#167, @pkgw)
