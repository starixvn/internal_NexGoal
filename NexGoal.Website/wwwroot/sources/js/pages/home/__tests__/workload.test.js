import React from "react";
import { shallow, mount } from "enzyme";
import WorkLoad from "../workload";
import renderer from 'react-test-renderer';
import axios from "axios";

jest.mock('axios');

const workloads = [
    { projectId: 1, name: 'HRM', totalHours: 10 },
    { projectId: 2, name: 'SMS HRM', totalHours: 14 },
    { projectId: 3, name: 'Nexgoal', totalHours: 21 },
    { projectId: 4, name: 'HRP', totalHours: 40 }
];

describe('Personal workload component', () => {
});