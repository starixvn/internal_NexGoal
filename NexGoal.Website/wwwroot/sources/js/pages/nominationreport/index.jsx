import React, { Component, Fragment } from 'react';
import ReactDOM from 'react-dom';
import { Tabs, Tab } from 'react-bootstrap';

import getResource from '../../resourceHelper';
import ToastLayout from '../../shared/layouts/ToastLayout';
import Breadcrumb from '../../shared/components/Breadcrumb';
import NominatorComment from './NominatorComment';
import EOQReport from './EOQReport';

export default class NominationReport extends Component {
    constructor(props) {
        super(props);

        this.state = {
            showModal: false,
            loadEOYTab: false,
        };
    }

    handleShowNominatorComment = (nomination, performanceRecordId, period) => {
        this.setState(prevState => {
            return {
                showModal: true,
                nomination: nomination,
                activeKey: performanceRecordId,
                period: period
            };
        });
    }

    handleSelectTab = (key) => {
        if (key === 2 && !this.state.loadEOYTab) {
            this.setState({ activeTabKey: key, loadEOYTab: true });
        } else {
            this.setState({ activeTabKey: key });
        }
    }

    closeModal = () => {
        this.setState({ showModal: false });
    }

    render() {
        return (
            <div className="row">
                <Breadcrumb>
                    <li className="current">{getResource('NominationReport')}</li>
                </Breadcrumb>
                <div className="col-sm-12">
                    <Tabs id="tabs" activeKey={this.state.activeTabKey} onSelect={this.handleSelectTab} animation={false}>
                        <Tab eventKey={1} title="Employee Of The Quarter">
                            <EOQReport isQuarter={true} onShowNominatorComment={this.handleShowNominatorComment} />
                        </Tab>
                        <Tab eventKey={2} title="Employee Of The Year">
                            {this.state.loadEOYTab && <EOQReport isQuarter={false} onShowNominatorComment={this.handleShowNominatorComment} />}
                        </Tab>
                    </Tabs>
                </div>
                <NominatorComment activeKey={this.state.activeKey} nomination={this.state.nomination} period={this.state.period} showModal={this.state.showModal} onCloseModal={this.closeModal} />
                <ToastLayout />
            </div>
        );
    }
}

ReactDOM.render(<NominationReport />, document.getElementById("nomination-report-wrapper"));