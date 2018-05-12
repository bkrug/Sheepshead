import * as React from 'react';
import { Route } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { GameSetup } from './components/setup/GameSetup';
import { RegisterHuman } from './components/setup/RegisterHuman';
import { GameFull } from './components/setup/GameFull';
import { Play } from './components/game/Play';

export const routes = <Layout>
    <Route exact path='/' component={ Home } />
    <Route path='/counter' component={ Counter } />
    <Route path='/fetchdata' component={ FetchData } />
    <Route path='/setup/create' component={ GameSetup } />
    <Route path='/setup/registerhuman' component={ RegisterHuman } />
    <Route path='/setup/gamefull' component={ GameFull } />
    <Route path='/game/play' component={ Play } />
</Layout>;
