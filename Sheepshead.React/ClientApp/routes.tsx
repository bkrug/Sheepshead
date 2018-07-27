import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { GameSetup } from './components/setup/GameSetup';
import { RegisterHuman } from './components/setup/RegisterHuman';
import { GameFull } from './components/setup/GameFull';
import { RegistrationWait } from './components/setup/RegistrationWait';
import { PlayPane } from './components/game/PlayPane';
import { Directions } from './components/directions/Directions';

export const routes =
<Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/setup/create' component={ GameSetup } />
    <Route path='/setup/registerhuman' component={ RegisterHuman } />
    <Route path='/setup/gamefull' component={ GameFull } />
    <Route path='/setup/registrationwait' component={RegistrationWait} />
    <Route path='/game/playpane' component={PlayPane} />
    <Route path='/directions' component={Directions} />
</Layout>;
