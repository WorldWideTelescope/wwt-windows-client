# rc: micro bump

No code changes from the previous release. Making a new release so that the
auto-update functionality can be exercised.


# WWTExplorer 6.0.905.0 (2022-01-06)

- This is why we have the betas: the previous couple of releases didn't get the
  auto-update functionality right. In this iteration, we revert the `msiexec`
  changes but use a newer version of Cranko that ought to (more) correctly
  update some of the installer metadata when making releases, which I believe
  will fix auto-update. With these changes the auto-update user experience isn't
  as smooth as it could be, since you have to go through steps like to
  re-confirming the installation directory, but it seems that there are
  significant limitations to getting the "smoother" mode to work reliably.


# WWTExplorer 6.0.904.0 (2022-01-05)

No code changes from the previous release. Making a new release so that the
auto-update functionality can be exercised.


# WWTExplorer 6.0.903.0 (2022-01-05)

- Attempt to fix the auto-updating functionality with current installers (#194,
  @pkgw). The old method of launching `msiexec /i` doesn't seem to work anymore,
  resulting in an error with a refusal to reinstall an already-installed
  application. We'll put out an additional release so that we can exercise the
  changed behavior (which now uses `msiexec /fvomus`).
- Make it so that the auto-update setting can be controlled from the command
  line, using `msiexec ... AUTOUPDATE=OFF` to disable auto-updates (#133, #194,
  @pkgw). This should be helpful for scripted installations. (All settings
  besides `OFF` should result in the default behavior of auto-updating being
  enabled.)


# WWTExplorer 6.0.902.0 (2022-01-04)

This release contains only non-functionality updates as we test the continuous
deployment infrastructure and release automation.

- Update some URLs embedded in the app
- Remove the "Tour Music/Resources" UI link, which we no longer (want to) support
- Update the location of `AutoUpdates` registry key to not mislead people about
  the creator of this software
- Update and improve the software license text shown in the installer
- Update the installer UI and metadata to try to be more correct
- Update the release automation infrastructure to include a bigger complement of
  preloaded data files in numbered releases (such as this one!)


# WWTExplorer 6.0.901.0 (2021-12-29)

No code changes from the previous release. Working on the continuous deployment
infrastructure.


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
