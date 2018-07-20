import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../../shared/layouts/ToastLayout';
import commonFunctions from '../../../common-functions';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import Settings from './Settings';

export default class App extends Component {
    constructor(props) {
        super(props);

        this.state = {
            activeKey: 1
        };

        this.handleSelect = this.handleSelect.bind(this);
    }

    handleSelect(key) {
        this.setState({ activeKey: key });
    }

    handleBackClick() {
        window.location = commonFunctions.getRootUrl() + 'NominationManagement/index?tab=eoy';
    }

    render() {
        return (
            <ToastLayout>
                <div className="row">
                    <Breadcrumb>
                        <li className="current">Nomination Management</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <a className="btn btn-cancel" onClick={this.handleBackClick}>
                            <i className="fa fa-angle-left"></i>Back to list
                        </a>
                    </div>
                    <div className="col-sm-12">
                        <Tabs id="tabs-nomination" animation={false}
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}>

                            <Tab eventKey={1} title="Settings">
                                <Settings />
                            </Tab>
                            <Tab eventKey={2} title="Role Management">
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('nomination-wrapper'));