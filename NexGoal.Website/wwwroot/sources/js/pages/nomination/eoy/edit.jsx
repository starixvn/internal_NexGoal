import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../../shared/layouts/ToastLayout';
import { resolveClientUrl } from '../../../common-functions';
import Breadcrumb from '../../../shared/components/Breadcrumb';
import Settings from './Settings';
import RoleManagement from './role/index';

export default class App extends Component {
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

    handleBackClick() {
        window.location = resolveClientUrl('Nomination/index?tab=eoy');
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
                            <i className="fa fa-angle-left" />Back to list
                        </a>
                    </div>
                    <div className="col-sm-12">
                        <Tabs id="tabs-nomination" animation={false}
                            activeKey={this.state.activeKey}
                            onSelect={this.handleSelect}
                        >

                            <Tab eventKey={1} title="Settings">
                                <Settings />
                            </Tab>
                            <Tab eventKey={2} title="Role Management">
                                {
                                    this.state.isFirstLoad &&
                                    <RoleManagement />
                                }
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </ToastLayout>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('nomination-wrapper'));