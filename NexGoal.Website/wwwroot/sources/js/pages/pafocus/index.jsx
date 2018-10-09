import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import getResource from '../../resourceHelper';

import PAFocus from './focus/index';
import Chart from './chart/index';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1,
            isFirstLoad: false
        };
    }

    handleSelect = (key) => {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">{`${getResource('ContributionAndRanking')} (${getResource('PAFocus')})`}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tabs id="tabs-pafocus"
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}
                        >
                            <Tab eventKey={1} title={getResource('PAFocus')}>
                                <PAFocus />
                            </Tab>
                            <Tab eventKey={2} title={getResource('Chart')}>
                                {
                                    this.state.isFirstLoad &&
                                    <Chart />
                                }
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('pafocus-wrapper'));