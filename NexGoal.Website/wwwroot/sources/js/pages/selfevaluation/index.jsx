import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import Breadcrumb from '../../shared/components/Breadcrumb';
import PerformanceCheckIn from './pci/index';
import PerformaceAppraisal from './PA';

export default class SelfEvaluation extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1,
            isFirstLoad: false
        };

        this.handleSelect = this.handleSelect.bind(this);
    }

    handleSelect(key) {
        this.setState({ activeKey: key });

        if (key === 2 && !this.state.isFirstLoad) {
            this.setState({ isFirstLoad: true });
        }
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">Self-Evaluation</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Tabs id="tab-self" animation={false}
                        activeKey={this.state.activeKey}
                        onSelect={this.handleSelect}>
                        <Tab title="My Performance Check-in" eventKey={1}>
                            <PerformanceCheckIn />
                        </Tab>
                        <Tab title="My Performace Appraisal" eventKey={2}>
                            {
                                this.state.isFirstLoad && <PerformaceAppraisal />
                            }
                        </Tab>
                    </Tabs>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<SelfEvaluation />, document.getElementById("self-evaluation-wrapper"));