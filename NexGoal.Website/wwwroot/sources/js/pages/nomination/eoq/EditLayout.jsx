import React, { PureComponent, Fragment } from 'react';
import { Tabs, Tab } from 'react-bootstrap';

import ToastLayout from '../../../shared/layouts/ToastLayout';
import getResource from '../../../resourceHelper';
import Breadcrumb from '../../../shared/components/Breadcrumb';

export default class EditLayout extends PureComponent {
    render() {
        return (
            <Fragment>
                <ToastLayout />
                <div className="row">
                    <Breadcrumb>
                        <li><a onClick={this.props.onBack}>{getResource('NominationPeriod')}</a></li>
                        <li className="current">{getResource('EditNominationPeriod')}</li>
                    </Breadcrumb>
                    <div className="col-sm-12">
                        <Tabs id="tabs-nomination"
                            activeKey={this.props.activeKey}
                            onSelect={this.props.onSelect}
                        >
                            <Tab eventKey={1} title={getResource('Settings')}>{this.props.settings}</Tab>
                            <Tab eventKey={2} title={getResource('RoleManagement')}>
                                {this.props.isFirstLoad && this.props.roleManagement}
                            </Tab>
                        </Tabs>
                    </div>
                </div>
            </Fragment>
        );
    }
}