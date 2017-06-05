import React from 'react';
import ReactDOM from 'react-dom';
import GameSetup from './GameSetup';
import Shuffle from './Shuffle'
import './index.css';
import { BrowserRouter as Router, Route } from 'react-router-dom'

ReactDOM.render(
    <Router>
        <div>
            <Route exact={true} path="/" component={GameSetup} />
            <Route path="/Shuffle" component={Shuffle} />
        </div>
    </Router>,
    document.getElementById('root')
);
