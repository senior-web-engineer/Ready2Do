import './materialize-src/sass/materialize.scss';
import './css/global-format.scss';
import './css/clienti.scss';
import Pica from 'pica/'

import { r2dEditEvento } from './js/r2do.EditEvento';
import { r2dRegistrazioneCliente } from './js/r2do.RegistrazioneCliente';
require('./js/ready2do');

const pica = Pica();
console.log("pica is:" + pica);
export { r2dEditEvento, pica, Pica }
