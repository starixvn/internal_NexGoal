import React, { Component } from "react";
import axios from 'axios';
import { axiosError, resolveClientUrl } from '../../common-functions';
import HorizontalTimeline from 'react-horizontal-timeline';
import TimelineLabel from './TimelineLabel';

export default class PersonalTimeline extends Component {
    constructor(props) {
        super(props);
        this.state = {
            PCIs: [],
            value: 0,
            previous: 0,
            isLabelHover: false
        };
    }

    componentDidMount = function () {
        this.getPCI();
    }

    getPCI = function () {
        axios.get(resolveClientUrl('Self/PerformanceCheckIn/Index'))
            .then(response => {
                this.setState(
                    {
                        PCIs: response.data.performanceRecords
                    });
            }).catch(axiosError);
    }

    render() {
        const values = this.state.PCIs.map((pci) => pci.createdDate);
        const label = (date, index) => {
            let pci = this.state.PCIs[index];
            if (pci) {
                return (
                    <TimelineLabel PCI={pci} />
                );
            }
        };
        return (
            <div style={{ height: '300px', margin: '0 auto', paddingTop: 20 }}>
                {
                    <HorizontalTimeline
                        index={this.state.value}
                        indexClick={(index) => {
                            this.setState({ value: index, previous: this.state.value });
                        }}
                        getLabel={label}
                        values={values}
                        fillingMotion={{ stiffness: 150, damping: 25 }}
                    />
                }
            </div>
        );
    }
} 